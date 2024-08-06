namespace Customers.Data.Domains;
public class CollectionUsersDto
{
    public int collection_users_id { get; set; }
    public int collection_id { get; set; }
    public int user_id { get; set; }
    public int created_by { get; set; }
    public DateTime created_on { get; set; }
}
