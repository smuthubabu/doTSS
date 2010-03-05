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
    /// Represents an user of the tpm framework
    /// </summary>
    public abstract class User
    {
        protected IDictionary<string, Group> _memberOf;

        /// <summary>
        /// Enumerates thoug all associated groups
        /// </summary>
        public IEnumerable<Group> MemberOf
        {
            get { return _memberOf.Values; }
        }

		/// <summary>
		/// Contains all Authentications that are supported
		/// </summary>
		protected IDictionary<string, Authentication> _authentications;
	
		/// <summary>
		/// Enumerates through all associated authentications
		/// </summary>
		public virtual IEnumerable<Authentication> Authentications
		{
			get{ return _authentications.Values; }
		}
		
        /// <summary>
        /// Gets the unique id of the user
        /// </summary>
        public abstract string Uid { get; }

        /// <summary>
        /// Gets the name of the user
        /// </summary>
        public abstract string Name { get; }

		/// <summary>
		/// Checks if the user is a member of group
		/// </summary>
		/// <param name="group">The group to check</param>
		/// <returns>
		/// </returns>
        public virtual bool IsMemberOf(Group group)
        {
            return _memberOf.ContainsKey(group.Gid);
        }
		
		/// <summary>
		/// Checks if the authentication method with the specified name is supported by this user
		/// </summary>
		/// <param name="name">Name to check</param>
		/// <returns>
		/// </returns>
		public virtual bool SupportsAuthentication(string name)
		{
			return _authentications.ContainsKey(name);
		}

		/// <summary>
		/// Checks if the specified authentication type is supported by this user
		/// </summary>
		/// <param name="type">type to check</param>
		/// <returns>
		/// </returns>
		public virtual bool SupportsAuthentication(Type type)
		{
			foreach(Authentication auth in Authentications)
			{
				if(type.IsAssignableFrom(auth.GetType()))
					return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Checks if the specified authentication method is supported by this user
		/// </summary>
		/// <returns>
		/// </returns>
		public virtual bool SupportsAuthentication<T>() where T: Authentication
		{
			return SupportsAuthentication(typeof(T));
		}
		
        public override bool Equals(object obj)
        {
            if (obj is User)
                return (obj as User).Uid.Equals(Uid);

            return false;
        }

        public override int GetHashCode()
        {
            return Uid.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Uid, Name);
        }

    }
}