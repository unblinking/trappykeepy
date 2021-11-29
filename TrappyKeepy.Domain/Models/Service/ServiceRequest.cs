namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Base service request class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceRequest<T>
    {
        public Guid? Id { get; set; }

        /// <summary>
        /// The item contains the details needed to complete the request.
        /// </summary>
        public T? Item { get; set; }
    }

    /// <summary>
    /// User service request.
    /// </summary>
    public class UserServiceRequest : ServiceRequest<User>
    {
        /// <summary>
        ///  If no user is provided, instantiate a new one.
        /// </summary>
        public UserServiceRequest()
        {
            this.Item = new User();
        }

        /// <summary>
        /// If a user is provided, set the user as the Item of the request.
        /// Example could be a request to create a new user.
        /// </summary>
        /// <param name="user"></param>
        public UserServiceRequest(User user)
        {
            this.Item = user;
        }

        public UserServiceRequest(Guid id)
        {
            this.Id = id;
        }
    }

    /// <summary>
    /// Keeper service request.
    /// </summary>
    public class KeeperServiceRequest : ServiceRequest<Keeper>
    {
        /// <summary>
        /// This holds the associated Filedata.BinaryData value.
        /// </summary>
        public byte[]? BinaryData { get; set; }

        /// <summary>
        ///  If no keeper is provided, instantiate a new one.
        /// </summary>
        public KeeperServiceRequest()
        {
            this.Item = new Keeper();
        }

        /// <summary>
        /// If a keeper is provided, set the keeper as the Item of the request.
        /// Example could be a request to create a new keeper.
        /// </summary>
        /// <param name="keeper"></param>
        public KeeperServiceRequest(Keeper keeper)
        {
            this.Item = keeper;
        }

        public KeeperServiceRequest(Guid id)
        {
            this.Id = id;
        }
    }

    /// <summary>
    /// Group service request.
    /// </summary>
    public class GroupServiceRequest : ServiceRequest<Group>
    {
        /// <summary>
        ///  If no group is provided, instantiate a new one.
        /// </summary>
        public GroupServiceRequest()
        {
            this.Item = new Group();
        }

        /// <summary>
        /// If a group is provided, set the group as the Item of the request.
        /// Example could be a request to create a new group.
        /// </summary>
        /// <param name="group"></param>
        public GroupServiceRequest(Group group)
        {
            this.Item = group;
        }

        public GroupServiceRequest(Guid id)
        {
            this.Id = id;
        }
    }

    /// <summary>
    /// Membership service request.
    /// </summary>
    public class MembershipServiceRequest : ServiceRequest<Membership>
    {
        /// <summary>
        /// The memberships table doesn't have an id colum.
        /// This can specify the group_id column value in a request.
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// The memberships table doesn't have an id colum.
        /// This can specify the user_id column value in a request.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        ///  If no membership is provided, instantiate a new one.
        /// </summary>
        public MembershipServiceRequest()
        {
            this.Item = new Membership();
        }

        /// <summary>
        /// If a membership is provided, set the membership as the Item of the request.
        /// Example could be a request to create a new membership.
        /// </summary>
        /// <param name="membership"></param>
        public MembershipServiceRequest(Membership membership)
        {
            this.Item = membership;
        }

        /// <summary>
        /// Used to request to delete a membership by groupId/userId.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        public MembershipServiceRequest(Guid groupId, Guid userId)
        {
            this.GroupId = groupId;
            this.UserId = userId;
        }
    }
}
