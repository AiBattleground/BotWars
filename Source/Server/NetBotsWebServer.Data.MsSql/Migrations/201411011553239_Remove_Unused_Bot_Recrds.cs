namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Unused_Bot_Recrds : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.BotRecords");
        }
        
        public override void Down()
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
            
        }
    }
}
