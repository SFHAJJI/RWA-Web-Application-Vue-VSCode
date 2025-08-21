USE [RWA]
GO
/****** Object:  StoredProcedure [dbo].[USP_HELIOS_POPULATE_CRR3]    Script Date: 2/27/2025 3:52:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[USP_HELIOS_POPULATE_CRR3]
	@Process nvarchar(256)
AS
BEGIN
	DECLARE @ErrorMssg nvarchar(3072),
			@ErrorCode nvarchar(1024)

	IF @Process IS NULL
		SET @Process = 'UPDATE Onglet Contrat Populate CRR3'

	SET NOCOUNT ON;
	BEGIN TRY

		--5.	Renseignement du champs ‘’ 48- Management Intent‘’ de l’onglet contrat du fichier MSI
		--5.1   Renseignement du champ pour les actifs qui ne sont pas traités en transparence (PRIO1)
		UPDATE [RWA].[dbo].[HELIOS_ONGLET_CONTRATS]
		SET [RWA].[dbo].[HELIOS_ONGLET_CONTRATS].[ManagementIntent] = b.ManagementIntent
		FROM [RWA].[dbo].[HELIOS_ONGLET_CONTRATS] a
		INNER JOIN [RWA].[dbo].[HELIOS_PARAM_MAPPING_TITRES_MAGNITUDE] b on a.IdentifiantContrat LIKE CONCAT('%', CONCAT(b.PartnerID, '%'))

		--6.	Renseignement du champ ‘’ LibelleTypeDette’’ de l’onglet contrat du fichier MSI
		--6.1.	Renseignement du champ pour les actifs qui ne sont pas traités en transparence (PRIO 1)
		UPDATE [RWA].[dbo].[HELIOS_ONGLET_CONTRATS]
		SET [RWA].[dbo].[HELIOS_ONGLET_CONTRATS].[LibelleTypeDette] = b.LibelleTypeDette
		FROM [RWA].[dbo].[HELIOS_ONGLET_CONTRATS] a
		INNER JOIN [RWA].[dbo].[HELIOS_PARAM_MAPPING_TITRES_MAGNITUDE] b on a.IdentifiantContrat LIKE CONCAT('%', CONCAT(b.PartnerID, '%'))
		WHERE a.CodeProduit in (SELECT CodeProduit FROM [RWA].[dbo].[HELIOS_PARAM_PERIMETRE_DETTE])

		--5.	Renseignement du champs ‘’ 48- Management Intent‘’ de l’onglet contrat du fichier MSI
		--5.2.	Renseignement du champ pour les actifs traités en transparence (PRIO 2)
		UPDATE [RWA].[dbo].[HELIOS_ONGLET_CONTRATS]
		SET [RWA].[dbo].[HELIOS_ONGLET_CONTRATS].[ManagementIntent] = 'LT'
		FROM [RWA].[dbo].[HELIOS_ONGLET_CONTRATS] a
		INNER JOIN [RWA].[dbo].[HELIOS_SEED_MONEY] b on SUBSTRING(a.IdentifiantContrat,1,3) = b.Source
		INNER JOIN [RWA].[dbo].[HELIOS_PARAM_MAPPING_TITRES_MAGNITUDE] c on b.PartnerID = c.PartnerID
		Where c.TitreLiquide = 'N'

		--6.	Renseignement du champ ‘’ LibelleTypeDette’’ de l’onglet contrat du fichier MSI
		--6.2.	Renseignement du champ pour les actifs traités en transparence (PRIO 2)
		UPDATE [RWA].[dbo].[HELIOS_ONGLET_CONTRATS]
		SET [RWA].[dbo].[HELIOS_ONGLET_CONTRATS].[LibelleTypeDette] = b.LibelleTypeDette
		FROM [RWA].[dbo].[HELIOS_ONGLET_CONTRATS] a
		INNER JOIN [RWA].[dbo].[HECATE_INTERNE_HISTORIQUE] b on a.IdentifiantContrat LIKE CONCAT('%', CONCAT(b.IdentifiantUniqueRetenu, '%')) or a.IdentifiantContrat LIKE CONCAT('%', CONCAT(b.IdentifiantOrigine, '%'))
		WHERE a.CodeProduit in (SELECT CodeProduit FROM [RWA].[dbo].[HELIOS_PARAM_PERIMETRE_DETTE])

	END TRY
	BEGIN CATCH
		-- En cas d'erreur lors du bloc précédent, on loggue
		SET @ErrorMssg = 'Erreur pendant l''update de [RWA].[dbo].[HELIOS_ONGLET_CONTRATS] Populate CRR3: ' + ERROR_MESSAGE()
		SET @ErrorCode = ERROR_NUMBER()
		EXEC dbo.USP_HELIOS_LOG_IMPORT_EXPORT 'INSERT', @Process, '', null, 2, @ErrorCode, @ErrorMssg
		PRINT '(' + @ErrorCode + ') - ' + @ErrorMssg
		RETURN @ErrorCode;
	END CATCH
END