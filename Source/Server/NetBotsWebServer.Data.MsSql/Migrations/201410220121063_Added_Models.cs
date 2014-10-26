namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Models : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BotRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Wins = c.Int(nullable: false),
                        Losses = c.Int(nullable: false),
                        Ties = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlayerBots",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Owner = c.String(),
                        URL = c.String(),
                        Record_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BotRecords", t => t.Record_Id)
                .Index(t => t.Record_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerBots", "Record_Id", "dbo.BotRecords");
            DropIndex("dbo.PlayerBots", new[] { "Record_Id" });
            DropTable("dbo.PlayerBots");
            DropTable("dbo.BotRecords");
        }
    }
}
