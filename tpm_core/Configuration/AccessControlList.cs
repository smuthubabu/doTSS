//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.Tpm.Configuration
{
    /// <summary>
    /// Base Access Control List configuration.
    /// </summary>
    public abstract class AccessControlList
    {
		
		protected IDictionary<string, User> _users;

        /// <summary>
        /// Gets all defined framework users
        /// 
        /// @key: user id
        /// </summary>
        protected virtual IDictionary<string, User> UsersById
		{ 
			get{ return _users; } 
		}

        /// <summary>
        /// Enumerates though the users
        /// </summary>
        public IEnumerable<User> Users
        {
            get{ return UsersById.Values; }
        }

		
		protected IDictionary<string, Group> _groups;
		
        /// <summary>
        /// Gets all defined framework groups
        /// </summary>
        protected virtual IDictionary<string, Group> GroupsById 
		{ 
			get{ return _groups; }
		}

        /// <summary>
        /// Enumerates through the groups
        /// </summary>
        public IEnumerable<Group> Groups
        {
            get { return GroupsById.Values; }
        }

		
		protected IDictionary<string, Permission> _permissions;
		
        /// <summary>
        /// Gets all defined permissions with its associated users and groups
        /// </summary>
        protected virtual IDictionary<string, Permission> Permissions 
		{ 
			get{ return _permissions; }
		}

        /// <summary>
        /// Looks for an users with the specified id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual User FindUserById(string userId)
        {
            IDictionary<string, User> users = UsersById;

            if (users.ContainsKey(userId))
                return users[userId];
            else
                return null;
        }
		
		/// <summary>
		/// Looks for a group with the specified id
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns>
		/// </returns>
		public virtual Group FindGroupById(string groupId)
		{
			IDictionary<string, Group> groups = GroupsById;
			
			if(groups.ContainsKey(groupId))
				return groups[groupId];
			else
				return null;
		}
		
		public virtual Permission FindPermission(string subsystem, string pid)
		{
			string uniquePermissionId = Permission.BuildUniquePermissionId(subsystem, pid);
			
			if(_permissions.ContainsKey(uniquePermissionId))
				return _permissions[uniquePermissionId];
			else
				return null;
		}
		
		/// <summary>
		/// Checks if the specified user has permission
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="pid"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public virtual bool IsUserAllowed(string subsystem, string pid, User user)
		{
			Permission permission = FindPermission(subsystem, pid);
			
			if(permission != null)
				return permission.IsUserPermitted(user);
			
			return false;
		}
		
		
		/// <summary>
		/// Checks if the specified user has permission
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="pid"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public virtual bool IsUserAllowed(string subsystem, string pid, ExternalUser user)
		{
			Permission permission = FindPermission(subsystem, pid);
			
			if(permission != null)
				return permission.IsExternalUserPermitted(user);
			
			return false;
		}
    }
}
