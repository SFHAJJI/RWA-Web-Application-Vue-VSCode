ALTER TABLE [RWA].[dbo].[HECATE_INTERNE_HISTORIQUE]
ADD BBGTicker nvarchar(30) NULL,
LibelleTypeDette nvarchar(12) NULL,
LastUpdate [nvarchar](40);

ALTER TABLE [RWA].[dbo].[HELIOS_PARAM_MAPPING_TITRES_MAGNITUDE]
ADD [LibelleTypeDette] [nvarchar](12) NULL,
[ManagementIntent] [nvarchar](2) NULL,
LastUpdate [nvarchar](40);

ALTER TABLE [RWA].[dbo].[HELIOS_ONGLET_CONTRATS]
ADD TauxProbabiliteDefaut float NULL,
ClasseLGD nvarchar(255) NULL,
PourcentageLoanToValueActualized nvarchar(3) NULL,
PourcentageLoanToValueOrigination nvarchar(3) NULL,
PourcentageLoanToValueCRR3 nvarchar(3) NULL,
ManagementIntent nvarchar(2) NULL,
LastUpdate varchar(40);

ALTER TABLE [RWA].[dbo].[HELIOS_ONGLET_CONTRATS_TEMP]
ADD TauxProbabiliteDefaut float NULL,
ClasseLGD nvarchar(255) NULL,
PourcentageLoanToValueActualized nvarchar(3) NULL,
PourcentageLoanToValueOrigination nvarchar(3) NULL,
PourcentageLoanToValueCRR3 nvarchar(3) NULL,
ManagementIntent nvarchar(2) NULL,
LastUpdate varchar(40);

ALTER TABLE [RWA].[dbo].[HELIOS_PARAM_MAPPING_TITRES_MAGNITUDE]
ADD CONSTRAINT CONTRAINTE_HELIOS_PARAM_MAPPING_TITRES_MAGNITUDE_LibelleTypeDette_INVALID CHECK (LibelleTypeDette IN ('SUBORDONNE')),
CONSTRAINT CONTRAINTE_HELIOS_PARAM_MAPPING_TITRES_MAGNITUDE_ManagementIntent_INVALID CHECK (ManagementIntent IN ('LT','ST'));

ALTER TABLE [RWA].[dbo].[HECATE_INTERNE_HISTORIQUE]  
ADD CONSTRAINT CONTRAINTE_HECATE_INTERNE_HISTORIQUE_LibelleTypeDette_INVALID2 CHECK (LibelleTypeDette IN ('SUBORDONNE'));

USE [RWA]
GO
/****** Object:  Table [dbo].[HELIOS_PARAM_PERIMETRE_DETTE]    Script Date: 01/26/2024 4:45:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HELIOS_PARAM_PERIMETRE_DETTE](
	[CodeProduit] [nvarchar](5) NOT NULL,
	[LibelleProduit] [nvarchar](80) NULL,
	[LastUpdate] [nvarchar](40) NOT NULL,
 CONSTRAINT [PK_HELIOS_PARAM_PERIMETRE_DETTE] PRIMARY KEY CLUSTERED 
(
	[CodeProduit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_HELIOS_PARAM_PERIMETRE_DETTE] ON dbo.[HELIOS_PARAM_PERIMETRE_DETTE] ([CodeProduit] ASC);
GO