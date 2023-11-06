﻿using Newtonsoft.Json.Linq;
using VedAstro.Library;

namespace Website;

public class PersonTools
{

    private readonly VedAstroAPI _api;

    private static List<Person> CachedPersonList { get; set; } = new List<Person>(); //if empty que to get new list

    /// <summary>
    /// public examples profiles always here if needed, list should never be empty, bad UX
    /// </summary>
    private static List<Person> CachedPublicPersonList { get; set; } = new List<Person>(); //if empty que to get new list

    //PUBLIC

    public PersonTools(VedAstroAPI vedAstroApi) => _api = vedAstroApi;

    /// <summary>
    /// getting people list is a long process, because of clean up and stuff
    /// so ask server to start prepare, will get results later when needed
    /// </summary>
    public void PreparePersonList()
    {
        //send the calls end of story, dont expect to check on it until needed let server handle it

        //get person list from server or cache and stores reference for later use
        AppData.API.Person.GetPersonList();
        AppData.API.Person.GetPublicPersonList();


    }

    /// <summary>
    /// person will be auto prepared, but might be slow
    /// as such prepare before hand if possible, like when app load
    /// </summary>
    public async Task<List<Person>> GetPersonList()
    {
        //CHECK CACHE
        //cache will be cleared when update is needed
        if (CachedPersonList.Any()) { return CachedPersonList; }

        //prepare url to call
        var url = $"{_api.URL.GetPersonList}/OwnerId/{_api.UserId}";
        var listNoPolling = await _api.GetListNoPolling(url, Person.FromJsonList);

        //NOTE: ToList is needed to make clone, else copies by ref and is lost
        CachedPersonList = listNoPolling.ToList();

        return CachedPersonList;
    }

    public async Task<List<Person>> GetPublicPersonList()
    {
        //CHECK CACHE
        //cache will be cleared when update is needed
        if (CachedPublicPersonList.Any()) { return CachedPublicPersonList; }

        //tell API to get started
        var url2 = $"{_api.URL.GetPersonList}/OwnerId/101/";
        var listNoPolling = await _api.GetListNoPolling(url2, Person.FromJsonList);

        //NOTE: ToList is needed to make clone, else copies by ref and is lost
        CachedPublicPersonList = listNoPolling.ToList();

        return CachedPublicPersonList;
    }

    /// <summary>
    /// Adds new person to API server main list
    /// </summary>
    public async Task<JToken> AddPerson(Person person)
    {
        //send newly created person to API server
        //var personJson = person.ToJson();
        //pass in user id to make sure user has right to delete
        //http://localhost:7071/api/AddPerson/OwnerId/234324x24/Name/Romeo/Gender/Female/Location/London/Time/13:45/01/06/1990
        var url = $"{_api.URL.AddPerson}/OwnerId/{_api.UserId}/Name/{person.Name}" +
                  $"/Gender/{person.Gender}" +
                  $"/Location/{Tools.RemoveWhiteSpace(person.GetBirthLocation().Name())}" +
                  $"/Time/{person.BirthHourMinute}/{person.BirthDateMonthYear}";
        var jsonResult = await Tools.WriteServer<JObject, JToken>(HttpMethod.Get, url);

#if DEBUG
        Console.WriteLine($"SERVER SAID:\n{jsonResult}");
#endif

        //if pass, clear local person cache
        await HandleResultClearLocalCache(person, jsonResult, "add");

        //up to caller to interpret data, can be failed one also
        return jsonResult;
    }

    /// <summary>
    /// Deletes person from API server  main list
    /// note:
    /// - takes care of pass and fail messages to end user
    /// - if fail will show alert message
    /// - cached person list is cleared here
    /// </summary>
    public async Task DeletePerson(Person personToDelete)
    {
        //tell API to get started
        //pass in user id to make sure user has right to delete
        var url = $"{_api.URL.DeletePerson}/OwnerId/{_api.UserId}/PersonId/{personToDelete.Id}";

        //API gives a url to check on poll fo results
        var jsonResult = await Tools.WriteServer<JObject, object>(HttpMethod.Get, url);

#if DEBUG
        Console.WriteLine($"SERVER SAID:\n{jsonResult}");
#endif

        //if pass, clear local person cache
        await HandleResultClearLocalCache(personToDelete, jsonResult, "delete"); //task is for message box

    }

    /// <summary>
    /// Send updated person to API server to be saved in main list
    /// note:
    /// - if fail will show alert message
    /// - cached person list is cleared here
    /// </summary>
    public async Task UpdatePerson(Person person)
    {
        //todo should check if local copy matches server before updating, cause could overwrite
        //todo detect first using async list if possible to see change from others or use versioning

        //prepare and send updated person to API server
        var updatedPerson = person.ToJson();
        var url = $"{_api.URL.UpdatePerson}";
        var jsonResult = await Tools.WriteServer<JObject, JToken>(HttpMethod.Post, url, updatedPerson);


#if DEBUG
        Console.WriteLine($"SERVER SAID:\n{jsonResult}");
#endif

        //if pass, clear local person cache
        await HandleResultClearLocalCache(person, jsonResult, "update");

    }

    /// <summary>
    /// used to get person direct not in users list for easy sharing
    /// </summary>
    public async Task<Person> GetPerson(string personId)
    {
        var url = $"{_api.URL.GetPerson}/OwnerId/{AppData.CurrentUser.Id}/PersonId/{personId}";
        var result = await Tools.ReadServerRaw<JObject>(url);

        //get parsed payload from raw result
        var person = VedAstroAPI.GetPayload(result, Person.FromJson);

        return person;
    }

    /// <summary>
    /// calls API to generate a new person ID, unique and human readable
    /// NOTE:
    /// - API has faster access to person list to cross refer, so done there and not in client
    /// - called before person new person is made on client
    /// </summary>
    public async Task<string> GetNewPersonId(string personName, int stdBirthYear)
    {
        //get all person profile owned by current user/visitor
        var url = $"{_api.URL.GetNewPersonId}/Name/{personName}/BirthYear/{stdBirthYear}";
        var jsonResult = await Tools.WriteServer<JObject, object>(HttpMethod.Get, url);

        //get parsed payload from raw result
        string personId = VedAstroAPI.GetPayload<string>(jsonResult, null);

        return personId;
    }


    //PRIVATE




    //---------------------------------------------PRIVATE
    /// <summary>
    /// checks status, if pass clears person list cache, for update, delete and add
    /// </summary>
    private async Task HandleResultClearLocalCache(Person personInQuestion, JToken jsonResult, string task)
    {

        //if anything but pass, raise alarm
        var status = jsonResult["Status"]?.Value<string>() ?? "";
        if (status != "Pass") //FAIL
        {
            var failMessage = jsonResult["Payload"]?.Value<string>() ?? "Server didn't give reason, pls try later.";
            await _api.ShowAlert("error", $"Server said no to your request! Why?", failMessage);
        }
        else //PASS
        {

            //1: clear stored person list
            PersonTools.CachedPersonList.Clear();

            //let user know person has been updates
            await _api.ShowAlert("success", $"{personInQuestion.Name} {task} complete!", false, timer: 1000);

        }
    }

}