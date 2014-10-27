namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserIdtoPlayerBot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerBots", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerBots", "UserId");
        }
    }
}
