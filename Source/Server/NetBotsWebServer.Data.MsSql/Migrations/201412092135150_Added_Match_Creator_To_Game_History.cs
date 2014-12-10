namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Match_Creator_To_Game_History : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameSummaries", "Initiater_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.GameSummaries", "Initiater_Id");
            AddForeignKey("dbo.GameSummaries", "Initiater_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameSummaries", "Initiater_Id", "dbo.AspNetUsers");
            DropIndex("dbo.GameSummaries", new[] { "Initiater_Id" });
            DropColumn("dbo.GameSummaries", "Initiater_Id");
        }
    }
}
