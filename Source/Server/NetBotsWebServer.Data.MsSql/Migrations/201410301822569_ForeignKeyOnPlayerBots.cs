namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKeyOnPlayerBots : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PlayerBots", new[] { "Owner_Id" });
            DropColumn("dbo.PlayerBots", "OwnerId");
            RenameColumn(table: "dbo.PlayerBots", name: "Owner_Id", newName: "OwnerId");
            AlterColumn("dbo.PlayerBots", "OwnerId", c => c.String(maxLength: 128));
            CreateIndex("dbo.PlayerBots", "OwnerId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PlayerBots", new[] { "OwnerId" });
            AlterColumn("dbo.PlayerBots", "OwnerId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.PlayerBots", name: "OwnerId", newName: "Owner_Id");
            AddColumn("dbo.PlayerBots", "OwnerId", c => c.Int(nullable: false));
            CreateIndex("dbo.PlayerBots", "Owner_Id");
        }
    }
}
