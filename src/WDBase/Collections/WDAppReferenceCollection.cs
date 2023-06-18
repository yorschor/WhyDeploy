// using System.Collections;
// using Newtonsoft.Json;
// using WDBase.Models;
//
// namespace WDBase.Collections;
//
// public class WDAppReferenceCollection : IEnumerable<DeployAppReference>
// {
//     private readonly List<DeployAppReference> apps;
//     public WDAppReferenceCollection(string json)
//     {
//          apps= JsonConvert.DeserializeObject<List<DeployAppReference>>(json) ?? new List<DeployAppReference>();
//     }
//     public IEnumerator<DeployAppReference> GetEnumerator()
//     {
//         return apps.GetEnumerator();
//     }
//
//     IEnumerator IEnumerable.GetEnumerator()
//     {
//         return GetEnumerator();
//     }
//
//     public void Add(DeployAppReference item)
//     {
//         apps.Add(item);
//     }
//
//     public bool Remove(DeployAppReference item)
//     {
//         return apps.Remove(item);
//     }
//     
//     public override string ToString()
//     {
//         return JsonConvert.SerializeObject(apps);
//     }
// }