using MyFWUnity.Core.Repositories;
using MyFWUnity.Core.Services;

using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.Module.Base.Repositories.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyFWUnity.DataAccess.Entity;

namespace MyFWUnity.Module.Base.Services.Default
{
    public class EntryRelationService : BaseService, IEntryRelationService
    {
        [Dependency]
        public IEntryRelationRepository EntryRelationRepository { get; set; }


        /// <summary>
        /// 根据关联ID和关联类型删除关系
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entryType"></param>
        public void DeleteEntryRelationByEntry(string id, string entryType)
        {
            IList<B_EntryRelation> entryRelations = EntryRelationRepository.FindList(n => n.EntryID == id && n.EntryType == entryType.ToString());
            if (entryRelations != null)
            {
                foreach (var entryRelation in entryRelations)
                {
                    EntryRelationRepository.Delete(entryRelation);
                }
                this.Context.Save();
            }
        }
        /// <summary>
        /// 根据关联ID和关联类型批量删除关系
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entryType"></param>
        public void DeleteEntryRelationsByEntry(string[] ids, string entryType)
        {
            IList<B_EntryRelation> entryRelations = EntryRelationRepository.FindList(n => ids.Contains(n.EntryID) && n.EntryType == entryType.ToString());
            if (entryRelations != null)
            {
                foreach (var entryRelation in entryRelations)
                {
                    EntryRelationRepository.Delete(entryRelation);
                }
                this.Context.Save();
            }
        }
        /// <summary>
        /// 获取关联关系ID 一对多关系
        /// </summary>
        /// <param name="entryType"></param>
        /// <param name="relationType"></param>
        /// <param name="relationID"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetEntryIDs(string entryType, string relationType, string relationID, string projectID = "")
        {
            Expression<Func<B_EntryRelation, bool>> expression = (n =>
              n.EntryType == entryType
              && n.RelationType == relationType
              && n.RelationID.Equals(relationID)
              && (n.ProjectID == projectID || n.ProjectID == null));
            List<B_EntryRelation> entryRelations = EntryRelationRepository.FindList(expression).ToList();
            if (entryRelations == null)
            {
                return null;
            }
            Dictionary<string, string> entryIDs = entryRelations.Select(n => new { id = n.ID, entryID = n.EntryID }).ToDictionary(n => n.entryID, n => n.id);
            return entryIDs;
        }

        /// <summary>
        /// 获取关联关系ID 多对多关系
        /// </summary>
        /// <param name="entryType"></param>
        /// <param name="relationType"></param>
        /// <param name="relationID"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetEntryIDs(string entryType, string relationType, string[] relationIDs, string projectID = "")
        {
            Expression<Func<B_EntryRelation, bool>> expression = (n =>
            n.EntryType == entryType
            && n.RelationType == relationType
            && relationIDs.Contains(n.RelationID)
            && (n.ProjectID == projectID || n.ProjectID == null));
            List<B_EntryRelation> entryRelations = EntryRelationRepository.FindList(expression).ToList();
            if (entryRelations == null)
            {
                return null;
            }
            Dictionary<string, string> entryIDs = entryRelations.Select(n => new { id = n.ID, entryID = n.EntryID }).ToDictionary(n => n.entryID, n => n.id);
            return entryIDs;
        }

        public string[] GetEntryRelationProjectByUserID(string entryType, string relationType, string relationID)
        {
            Expression<Func<B_EntryRelation, bool>> expression = (n =>
            n.EntryType == entryType
            && n.RelationType == relationType
            && n.RelationID.Equals(relationID));
            List<B_EntryRelation> entryRelations = EntryRelationRepository.FindList(expression).ToList();
            if (entryRelations == null)
            {
                return null;
            }
            string[] projectIDs = entryRelations.Select(n => n.ProjectID).ToArray();
            return projectIDs;
        }
    }
}
