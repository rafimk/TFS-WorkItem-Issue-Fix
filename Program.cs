
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TFS_WorkItem_Issue_Fix;

var notString = "";
var token = "";

using (HttpClient client = new HttpClient())
{
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{notString}:{token}")));
    
    using (HttpResponseMessage response = await client.GetAsync("http://tfs/tfs/TestCollection/supplychain/_apis/wit/workitems?ids=1&api-version=6.0"))
    {
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var jsonTFSObject = JObject.Parse(responseBody);

        notString = (string)jsonTFSObject["fields"]["System.Description"];
    }

    Console.WriteLine(notString);
}

var goodString = WebUtility.HtmlEncode(notString);
using (HttpClient client = new HttpClient())
{
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{notString}:{token}")));
    
    List<TFSContract> patchDocument = new List<TFSContract>();
    patchDocument.Add(new TFSContract { op = "add", path = "/fields/System.Description", value = goodString });

    var jsonPatchDocument = JsonConvert.SerializeObject(patchDocument);

    StringContent content = new StringContent(jsonPatchDocument, Encoding.UTF8, "application/json-patch+json");

    var httpPatch = await client.PatchAsync("http://tfs/tfs/TestCollection/supplychain/_apis/wit/workitems/1?api-version=6.0", content);

   httpPatch.EnsureSuccessStatusCode();
}


