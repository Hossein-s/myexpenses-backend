CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Categories" (
    "Id" uuid NOT NULL,
    "Name" text NULL,
    CONSTRAINT "PK_Categories" PRIMARY KEY ("Id")
);

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Email" text NULL,
    "FirstName" text NULL,
    "LastName" text NULL,
    "Password" text NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Expenses" (
    "Id" uuid NOT NULL,
    "Amount" numeric NOT NULL,
    "UserId" uuid NULL,
    "SpentAt" timestamp with time zone NOT NULL,
    "CategoryId" uuid NULL,
    CONSTRAINT "PK_Expenses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Expenses_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id"),
    CONSTRAINT "FK_Expenses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id")
);

CREATE TABLE "Incomes" (
    "Id" uuid NOT NULL,
    "Amount" numeric NOT NULL,
    "UserId" uuid NULL,
    "RecievedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Incomes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Incomes_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id")
);

CREATE TABLE "RefreshTokens" (
    "Id" uuid NOT NULL,
    "Token" text NULL,
    "UserId" uuid NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_RefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id")
);

CREATE INDEX "IX_Expenses_CategoryId" ON "Expenses" ("CategoryId");

CREATE INDEX "IX_Expenses_UserId" ON "Expenses" ("UserId");

CREATE INDEX "IX_Incomes_UserId" ON "Incomes" ("UserId");

CREATE INDEX "IX_RefreshTokens_UserId" ON "RefreshTokens" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230318122532_InitialCreate', '7.0.3');

COMMIT;


