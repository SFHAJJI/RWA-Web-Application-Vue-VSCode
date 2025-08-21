
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'RWA_TRACE') AND type in (N'U'))
DROP TABLE RWA_TRACE
GO

CREATE TABLE RWA_TRACE(
	LOGIN nvarchar(250) NULL,
	SECTION nvarchar(250) NULL,
	STEP nvarchar(250) NULL,
	INFO nvarchar(250) NULL,
	DT datetime NOT NULL DEFAULT getdate()
	) ON [PRIMARY]
GO

