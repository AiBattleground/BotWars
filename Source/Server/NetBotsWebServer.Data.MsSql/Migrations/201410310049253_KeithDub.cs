namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KeithDub : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GameSummaries", "PlayerBot_Id", "dbo.PlayerBots");
            DropIndex("dbo.GameSummaries", new[] { "PlayerBot_Id" });
            DropColumn("dbo.GameSummaries", "PlayerBot_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GameSummaries", "PlayerBot_Id", c => c.Int());
            CreateIndex("dbo.GameSummaries", "PlayerBot_Id");
            AddForeignKey("dbo.GameSummaries", "PlayerBot_Id", "dbo.PlayerBots", "Id");
        }
    }
}
