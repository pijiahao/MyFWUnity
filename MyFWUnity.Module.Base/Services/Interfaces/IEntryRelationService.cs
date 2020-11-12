using MyFWUnity.Core.Services;
using MyFWUnity.Module.Base.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.Services.Interfaces
{
    public interface IEntryRelationService : IBaseService
    {
        void DeleteEntryRelationByEntry(string id, string entryType);
        void DeleteEntryRelationsByEntry(string[] ids, string entryType);
        Dictionary<string, string> GetEntryIDs(string entryType, string relationType, string relationID, string projectID = "");
        Dictionary<string, string> GetEntryIDs(string entryType, string relationType, string[] sourceIDs, string projectID = "");
        string[] GetEntryRelationProjectByUserID(string entryType, string relationType, string relationID);
    }
}
