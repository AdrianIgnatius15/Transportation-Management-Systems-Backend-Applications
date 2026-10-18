CREATE SCHEMA tms;


ALTER SCHEMA tms OWNER TO tms;

COMMENT ON SCHEMA tms IS 'This database becomes the main database for Transportation Management Systems';


CREATE TABLE tms."Addresses" (
    "Id" uuid NOT NULL,
    "Line1" text NOT NULL,
    "Line2" text,
    "City" text NOT NULL,
    "State" text NOT NULL,
    "PostalCode" text NOT NULL,
    "Country" text NOT NULL,
    "Latitude" double precision,
    "Longitude" double precision
);


ALTER TABLE tms."Addresses" OWNER TO tms;

CREATE TABLE tms."Clients" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "ContactEmail" text NOT NULL,
    "ContactPhone" text NOT NULL
);


ALTER TABLE tms."Clients" OWNER TO tms;

CREATE TABLE tms."Documents" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "Type" integer NOT NULL,
    "FileName" text NOT NULL,
    "StorageUrl" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE tms."Documents" OWNER TO tms;

CREATE TABLE tms."Orders" (
    "Id" uuid NOT NULL,
    "ClientId" uuid NOT NULL,
    "OrderNumber" text NOT NULL,
    "Status" integer NOT NULL,
    "Priority" text NOT NULL,
    "ShipmentAddressId" uuid NOT NULL,
    "DeliveryAddressId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE tms."Orders" OWNER TO tms;

CREATE TABLE tms."Pieces" (
    "Id" uuid NOT NULL,
    "Description" text NOT NULL,
    "Weight" numeric NOT NULL,
    "Height" numeric NOT NULL,
    "Width" numeric NOT NULL,
    "Length" numeric NOT NULL,
    "ShipmentId" uuid NOT NULL
);


ALTER TABLE tms."Pieces" OWNER TO tms;

CREATE TABLE tms."Shipments" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL
);


ALTER TABLE tms."Shipments" OWNER TO tms;

CREATE TABLE tms."TrackingEvents" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "EventType" integer NOT NULL,
    "Description" text,
    "Latitude" double precision,
    "Longitude" double precision,
    "Timestamp" timestamp with time zone NOT NULL
);


ALTER TABLE tms."TrackingEvents" OWNER TO tms;

CREATE TABLE tms."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE tms."__EFMigrationsHistory" OWNER TO tms;

CREATE TABLE tms.databasechangeloglock (
    id integer NOT NULL,
    locked boolean NOT NULL,
    lockgranted timestamp without time zone,
    lockedby character varying(255)
);


ALTER TABLE tms.databasechangeloglock OWNER TO tms;

COPY tms."Addresses" ("Id", "Line1", "Line2", "City", "State", "PostalCode", "Country", "Latitude", "Longitude") FROM stdin;
ac522099-ec95-445b-a3d0-642a0550886e	426 Bucknell Road		Costa Mesa	California	92626	United States	33.6646092	-117.9169761
c4e6e479-36aa-40f4-aead-9c962aa6e3d0	12141 Lewis Street		Garden Grove	California	92840	United States	33.7873848	-117.8989045
b9426a52-9435-4598-a836-3bc3e69a6a3b	426 Bucknell Road		Costa Mesa	California	92626	United States	33.6646089	-117.916999
d1e44de0-d63d-4d0b-b77f-b93b23400bff	12141 Lewis Street		Garden Grove	California	92840	United States	33.7873848	-117.8989045
\.

COPY tms."Clients" ("Id", "Name", "ContactEmail", "ContactPhone") FROM stdin;
7afa16e1-6d65-44dd-8822-bdc10fe6347f	Abraham Johnson	tms-customer@dhl.com	+6017-2477348
\.

COPY tms."Documents" ("Id", "OrderId", "Type", "FileName", "StorageUrl", "CreatedAt") FROM stdin;
\.

COPY tms."Orders" ("Id", "ClientId", "OrderNumber", "Status", "Priority", "ShipmentAddressId", "DeliveryAddressId", "CreatedAt", "UpdatedAt") FROM stdin;
b65f0228-c150-46b6-b54e-c0d96efc70aa	7afa16e1-6d65-44dd-8822-bdc10fe6347f	eceee27f-8e86-4e17-ada4-754b5cd20f2a	0	Standard	b9426a52-9435-4598-a836-3bc3e69a6a3b	d1e44de0-d63d-4d0b-b77f-b93b23400bff	2026-09-11 14:13:05.45912+00	2026-09-11 14:13:05.459154+00
\.

COPY tms."Pieces" ("Id", "Description", "Weight", "Height", "Width", "Length", "ShipmentId") FROM stdin;
a5efd3f8-4441-4184-9a31-5335139fea0a	Communion Wafers	50	12	12	12	73619a7e-302e-4ce8-b519-3fd52964cb1f
aeb7dd01-cafa-4b00-9c3e-fe678e4bb4cf	Priests Stoles	25	12	23	12	73619a7e-302e-4ce8-b519-3fd52964cb1f
\.

COPY tms."Shipments" ("Id", "OrderId") FROM stdin;
73619a7e-302e-4ce8-b519-3fd52964cb1f	b65f0228-c150-46b6-b54e-c0d96efc70aa
\.

COPY tms."TrackingEvents" ("Id", "OrderId", "EventType", "Description", "Latitude", "Longitude", "Timestamp") FROM stdin;
\.

COPY tms."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260621112111_InitialMigrationToNewDB	10.0.4
20260712112517_ConvertTrackingEventsToHypertable	10.0.9
\.

COPY tms.databasechangeloglock (id, locked, lockgranted, lockedby) FROM stdin;
1	f	\N	\N
1000	f	\N	\N
\.

ALTER TABLE tms."Addresses"
    ADD CONSTRAINT "PK_Addresses" PRIMARY KEY ("Id");

ALTER TABLE tms."Clients"
    ADD CONSTRAINT "PK_Clients" PRIMARY KEY ("Id");

ALTER TABLE tms."Documents"
    ADD CONSTRAINT "PK_Documents" PRIMARY KEY ("Id");

ALTER TABLE tms."Orders"
    ADD CONSTRAINT "PK_Orders" PRIMARY KEY ("Id");

ALTER TABLE tms."Pieces"
    ADD CONSTRAINT "PK_Pieces" PRIMARY KEY ("Id");

ALTER TABLE tms."Shipments"
    ADD CONSTRAINT "PK_Shipments" PRIMARY KEY ("Id");

ALTER TABLE tms."TrackingEvents"
    ADD CONSTRAINT "PK_TrackingEvents" PRIMARY KEY ("Id", "Timestamp");

ALTER TABLE tms."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");

ALTER TABLE tms.databasechangeloglock
    ADD CONSTRAINT databasechangeloglock_pkey PRIMARY KEY (id);

CREATE INDEX "IX_Documents_OrderId" ON tms."Documents" USING btree ("OrderId");

CREATE INDEX "IX_Orders_ClientId" ON tms."Orders" USING btree ("ClientId");

CREATE INDEX "IX_Orders_DeliveryAddressId" ON tms."Orders" USING btree ("DeliveryAddressId");

CREATE INDEX "IX_Orders_ShipmentAddressId" ON tms."Orders" USING btree ("ShipmentAddressId");

CREATE INDEX "IX_Pieces_ShipmentId" ON tms."Pieces" USING btree ("ShipmentId");

CREATE INDEX "IX_Shipments_OrderId" ON tms."Shipments" USING btree ("OrderId");

CREATE INDEX "IX_TrackingEvents_OrderId" ON tms."TrackingEvents" USING btree ("OrderId");

CREATE INDEX "TrackingEvents_Timestamp_idx" ON tms."TrackingEvents" USING btree ("Timestamp" DESC);

ALTER TABLE tms."Documents"
    ADD CONSTRAINT "FK_Documents_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES tms."Orders"("Id") ON DELETE CASCADE;

ALTER TABLE tms."Orders"
    ADD CONSTRAINT "FK_Orders_Addresses_DeliveryAddressId" FOREIGN KEY ("DeliveryAddressId") REFERENCES tms."Addresses"("Id") ON DELETE CASCADE;

ALTER TABLE tms."Orders"
    ADD CONSTRAINT "FK_Orders_Addresses_ShipmentAddressId" FOREIGN KEY ("ShipmentAddressId") REFERENCES tms."Addresses"("Id") ON DELETE CASCADE;

ALTER TABLE tms."Orders"
    ADD CONSTRAINT "FK_Orders_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES tms."Clients"("Id") ON DELETE CASCADE;

ALTER TABLE tms."Pieces"
    ADD CONSTRAINT "FK_Pieces_Shipments_ShipmentId" FOREIGN KEY ("ShipmentId") REFERENCES tms."Shipments"("Id") ON DELETE CASCADE;

ALTER TABLE tms."Shipments"
    ADD CONSTRAINT "FK_Shipments_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES tms."Orders"("Id") ON DELETE CASCADE;

ALTER TABLE tms."TrackingEvents"
    ADD CONSTRAINT "FK_TrackingEvents_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES tms."Orders"("Id") ON DELETE CASCADE;
