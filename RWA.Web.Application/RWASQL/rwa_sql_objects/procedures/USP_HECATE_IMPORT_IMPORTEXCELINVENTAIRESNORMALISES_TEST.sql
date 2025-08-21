USE [RWA]
GO
/****** Object:  StoredProcedure [dbo].[USP_HECATE_IMPORT_IMPORTEXCELINVENTAIRESNORMALISES_TEST]    Script Date: 03/31/25 2:56:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[USP_HECATE_IMPORT_IMPORTEXCELINVENTAIRESNORMALISES_TEST]
	-- Add the parameters for the stored procedure here
	@PeriodeCloture nvarchar(6),	--'122016'
	@Source nvarchar(3),			--'061'
	@NomFichierXLSX nvarchar(255),	--RWA_Report_066_062016.xlsx
	@OngletFichierXLSX nvarchar(30) --RWA_Report
AS
BEGIN

DECLARE @Identifiant nvarchar(50),
		@Nom nvarchar(255),
		@Categorie1 nvarchar(100),
		@Categorie2 nvarchar(100),
		@DateMaturite date,
		@DateExpiration date,
		@RAF nvarchar(7),
		
		@ValeurDeMarche float,
		@DeviseDeCotation nvarchar(30),
		@TauxObligation decimal(18,2),
		@Tiers nvarchar(255),
		@BOA_SJ nvarchar(7),
		@BOA_Contrepartie nvarchar(7),
		@BOA_DEFAUT nvarchar(7),
		
		@Enrichi_RefCategorieRWA nvarchar(3),
		@Enrichi_IdentifiantUniqueRetenu nvarchar(13),
		@Enrichi_RAF nvarchar(7),
		@Enrichi_LibelleOrigine nvarchar(255),
		@Enrichi_DateFinContrat date,
		@Enrichi_IdentifiantOrigine nvarchar(50),
		@Enrichi_Commentaire nvarchar(200),
		@Enrichi_Bloomberg nvarchar(70),
		
		@CodeResult int,
		
		@start datetime, 
		@stop datetime,
		@sqlCommand nvarchar(3000),
		
		@exist int, 
		@ChaineTMP nvarchar(1000),
		@ChaineTMP2 nvarchar(1000),
		
		@NumLigne int=0,
		@nbrLigne int,
		
		@LigneCONSO int=0,
		@LigneREUSSI int=0,
		@LigneECHEC int=0,
		@LigneEXCLU int=0
		
	
	
	SET @ChaineTMP='E:\RWA\HECATE\Import\Consolidation\' + @NomFichierXLSX
	EXEC TraceRwa 'xp_fileexist', 'IN', @NomFichierXLSX;
	EXEC xp_fileexist @ChaineTMP, @exist OUTPUT
	EXEC TraceRwa 'xp_fileexist', 'OUT', @NomFichierXLSX;
	
	IF @exist=1 AND @PeriodeCloture is not null AND @Source is not null AND @NomFichierXLSX is not null AND @OngletFichierXLSX is not null
	BEGIN
		EXEC TraceRwa 'PeriodeCloture', 'IN', @PeriodeCloture;
		DELETE FROM HECATE_COMPTEUR
		WHERE PeriodeCloture=@PeriodeCloture AND [Source]=@Source
		
		DELETE FROM dbo.HECATE_LOG_CONSO
		WHERE PeriodeCloture=@PeriodeCloture AND [Source]=@Source
		
		DELETE FROM dbo.HECATE_INVENTAIRE_NORMALISES
		WHERE PeriodeCloture=@PeriodeCloture AND [Source]=@Source
		
		DELETE FROM dbo.HECATE_LOG_IMPORT_EXPORT
		WHERE Process='IMPORT INV NORM' AND PeriodeCloture=@PeriodeCloture AND [Source]=@Source
		
		EXEC TraceRwa 'PeriodeCloture', 'DROP TEMP_DATA';
		IF OBJECT_ID('dbo.TEMP_DATA') is not null
		DROP TABLE TEMP_DATA
		
		SET @sqlCommand = N'SELECT cast(rtrim(ltrim([Asset ID])) as nvarchar(50)) as Identifiant,
      cast(rtrim(ltrim([Asset Description])) as nvarchar(255)) as Nom,
      CONVERT(float,[Market Value])as ValeurDeMarche,
      cast(rtrim(ltrim([Asset Type 1])) as nvarchar(100)) as Categorie1,
      cast(rtrim(ltrim([Asset Type 2])) as nvarchar(100))as Categorie2,
		CASE
			WHEN [Local Currency] IS NULL THEN ''EUR''
			WHEN LEN(cast(rtrim(ltrim([Local Currency])) as nvarchar(50)))<>3 THEN ''EUR''
			ELSE cast(rtrim(ltrim([Local Currency])) as nvarchar(50)) 
		END as DeviseDeCotation,
        CASE 
		WHEN rtrim(ltrim([Obligation Rate])) = ''-'' THEN 0
		WHEN rtrim(ltrim(REPLACE([Obligation Rate],char(13),'' '')))='''' THEN  null ELSE convert(decimal(18,2),[Obligation Rate]) END as TauxObligation,
        CASE 
            WHEN ISDATE([Maturity Date])=1 THEN [Maturity Date]
            WHEN CONVERT(date,[Maturity Date],103)=''1900-01-01'' THEN NULL
            ELSE CONVERT(date,[Maturity Date],103) 
        END
        AS DateMaturite,
      CASE 
            WHEN ISDATE([Expiration Date])=1 THEN [Expiration Date]
            WHEN CONVERT(date,[Expiration Date],103)=''1900-01-01'' THEN NULL
            ELSE CONVERT(date,[Expiration Date],103) 
        END
         as DateExpiration,
      cast(rtrim(ltrim([Counterparty])) as nvarchar(255)) as Tiers,
      cast(rtrim(ltrim([RAF])) as nvarchar(7)) as RAF,
      cast(rtrim(ltrim([BOA_SJ])) as nvarchar(7)) as BOA_SJ,
      cast(rtrim(ltrim([BOA_Contrepartie])) as nvarchar(7)) as BOA_Contrepartie,
      cast(rtrim(ltrim([BOA_DEFAUT])) as nvarchar(7)) as BOA_DEFAUT into TEMP_DATA
		FROM OPENROWSET (	''Microsoft.ACE.OLEDB.12.0'',
							''Excel 12.0;Database=E:\RWA\HECATE\Import\Consolidation\' + @NomFichierXLSX + ';HDR=YES;IMEX=1'',
							''SELECT * FROM [' + @OngletFichierXLSX + '$]'') WHERE [Asset ID] is not null AND [Asset Description] is not null'
		BEGIN TRY
				EXEC TraceRwa 'OPENROWSET', 'IN';
			EXEC sp_executesql @sqlCommand;
			SET @nbrLigne = @@ROWCOUNT
				EXEC TraceRwa 'OPENROWSET', 'nbrLigne', @nbrLigne;
			IF @nbrLigne=0
			BEGIN
				SET @ChaineTMP = 'L''onglet ('+@OngletFichierXLSX+') est vide.'
				EXEC dbo.USP_HECATE_LOG_SETLOGIMPORTEEXPORT 2,@PeriodeCloture,@Source,'IMPORT INV NORM',1000,@ChaineTMP
				PRINT @ChaineTMP
				RETURN 1000;
			END
			--SELECT @nbrLigne=COUNT(*) FROM TEMP_DATA WHERE LEN(DeviseDeCotation)<>3
			--IF @nbrLigne>0
			--BEGIN
			--	SET @ChaineTMP = 'Le fichier '+@NomFichierXLSX+' contient au moins une devise vide.'
			--	EXEC dbo.USP_HECATE_LOG_SETLOGIMPORTEEXPORT 2,@PeriodeCloture,@Source,'IMPORT INV NORM',1000,@ChaineTMP
			--	PRINT @ChaineTMP
			--	RETURN 1001;
			--END
		END TRY
		BEGIN CATCH
			SET @ChaineTMP = 'ERREUR FICHIER EXCEL: ' + ERROR_MESSAGE()
			SET @ChaineTMP2= ERROR_NUMBER()
			EXEC dbo.USP_HECATE_LOG_SETLOGIMPORTEEXPORT 2,@PeriodeCloture,@Source,'IMPORT INV NORM',@ChaineTMP2,@ChaineTMP
			
			PRINT '('+ @ChaineTMP2+ ') - ' + @ChaineTMP
			
			RETURN @ChaineTMP2;
		END CATCH
		
				EXEC TraceRwa 'CONTROLEINTEGRATION', 'IN';
		EXEC @CodeResult=dbo.USP_HECATE_TRAITEMENT_CONTROLEINTEGRATION @PeriodeCloture, @Source
				EXEC TraceRwa 'CONTROLEINTEGRATION', 'OUT', @CodeResult;
		IF @CodeResult <> 0
		BEGIN
			DROP TABLE TEMP_DATA;
			RETURN @CodeResult;
		END
		
		SET @start = GETDATE();
		
		DECLARE CursorTest CURSOR FOR Select Identifiant, Nom, Categorie1, Categorie2, DateMaturite, DateExpiration, RAF, ValeurDeMarche, DeviseDeCotation, TauxObligation, Tiers, BOA_SJ, BOA_Contrepartie, BOA_DEFAUT  FROM TEMP_DATA
		
		OPEN CursorTest
		
				EXEC TraceRwa 'CURSOR', 'IN';
		
			FETCH NEXT FROM CursorTest INTO @Identifiant, @Nom, @Categorie1, @Categorie2, @DateMaturite, @DateExpiration, @RAF, @ValeurDeMarche, @DeviseDeCotation, @TauxObligation, @Tiers, @BOA_SJ, @BOA_Contrepartie, @BOA_DEFAUT
			
				EXEC TraceRwa 'CURSOR', 'NEXT';
			
			WHILE @@FETCH_STATUS=0
			BEGIN
				SET @Enrichi_RefCategorieRWA=null
				SET	@Enrichi_IdentifiantUniqueRetenu=null
				SET	@Enrichi_RAF=null
				SET	@Enrichi_LibelleOrigine=null
				SET	@Enrichi_DateFinContrat=null
				SET	@Enrichi_IdentifiantOrigine=null
				SET	@Enrichi_Commentaire=null
				SET	@Enrichi_Bloomberg=null
				
				IF rtrim(ltrim(@RAF))<>''
					SET @RAF = RIGHT('0000000'+CAST(rtrim(ltrim(@RAF)) AS VARCHAR(7)),7)
				ELSE
					SET @RAF = null
				
				EXEC TraceRwa 'CURSOR', 'WORK', @RAF;
					
				IF @ValeurDeMarche < -0.1 OR @ValeurDeMarche > 0.1
				BEGIN
					SET @LigneCONSO = @LigneCONSO+1
					
				EXEC TraceRwa 'CURSOR', 'WORK-CONSOLIDE';
				
					EXEC @CodeResult=dbo.USP_HECATE_TRAITEMENT_CONSOLIDE @LigneCONSO,@PeriodeCloture,@Source,@Identifiant,@Nom,@Categorie1,@Categorie2, @DateMaturite, @DateExpiration, @RAF, @Enrichi_RefCategorieRWA OUTPUT, @Enrichi_IdentifiantUniqueRetenu OUTPUT, @Enrichi_RAF OUTPUT, @Enrichi_LibelleOrigine OUTPUT, @Enrichi_DateFinContrat OUTPUT, @Enrichi_IdentifiantOrigine OUTPUT, @Enrichi_Commentaire OUTPUT, @Enrichi_Bloomberg OUTPUT
					
				EXEC TraceRwa 'CURSOR', 'WORK-CONSOLIDE', @CodeResult;
					IF (@CodeResult>400 and @CodeResult<=499) OR (@CodeResult>600 and @CodeResult<=699)
					BEGIN -- Ligne réalisé
						IF (@CodeResult>400 and @CodeResult<=499)
							EXEC dbo.USP_HECATE_TRAITEMENT_SETINVENTAIRESNORMALISES @LigneCONSO,@PeriodeCloture,@Source,@Identifiant,@Nom,@ValeurDeMarche,@Categorie1,@Categorie2,@DeviseDeCotation,@TauxObligation,@DateMaturite,@DateExpiration,@Tiers,@RAF,@BOA_SJ,@BOA_Contrepartie,@BOA_DEFAUT,@Identifiant, @Enrichi_RefCategorieRWA, @Enrichi_IdentifiantUniqueRetenu, @Enrichi_RAF, @Enrichi_LibelleOrigine, @Enrichi_DateFinContrat, @Enrichi_Commentaire, @Enrichi_Bloomberg,3, 1, @CodeResult
							
						IF (@CodeResult>600 and @CodeResult<=699)
							EXEC dbo.USP_HECATE_TRAITEMENT_SETINVENTAIRESNORMALISES @LigneCONSO,@PeriodeCloture,@Source,@Identifiant,@Nom,@ValeurDeMarche,@Categorie1,@Categorie2,@DeviseDeCotation,@TauxObligation,@DateMaturite,@DateExpiration,@Tiers,@RAF,@BOA_SJ,@BOA_Contrepartie,@BOA_DEFAUT,@Identifiant, @Enrichi_RefCategorieRWA, @Enrichi_IdentifiantUniqueRetenu, @Enrichi_RAF, @Enrichi_LibelleOrigine, @Enrichi_DateFinContrat, @Enrichi_Commentaire, @Enrichi_Bloomberg,4, 1, @CodeResult
							
						SET @LigneREUSSI = @LigneREUSSI+1
					END
					ELSE
					BEGIN -- Ligne non réalisé
						IF (@CodeResult>100 and @CodeResult<=199)
							EXEC dbo.USP_HECATE_TRAITEMENT_SETINVENTAIRESNORMALISES @LigneCONSO,@PeriodeCloture,@Source,@Identifiant,@Nom,@ValeurDeMarche,@Categorie1,@Categorie2,@DeviseDeCotation,@TauxObligation,@DateMaturite,@DateExpiration,@Tiers,@RAF,@BOA_SJ,@BOA_Contrepartie,@BOA_DEFAUT,@Identifiant, @Enrichi_RefCategorieRWA, @Enrichi_IdentifiantUniqueRetenu, @Enrichi_RAF, @Enrichi_LibelleOrigine, @Enrichi_DateFinContrat, @Enrichi_Commentaire, @Enrichi_Bloomberg,1, 2, @CodeResult
							
						IF (@CodeResult>200 and @CodeResult<=299)
							EXEC dbo.USP_HECATE_TRAITEMENT_SETINVENTAIRESNORMALISES @LigneCONSO,@PeriodeCloture,@Source,@Identifiant,@Nom,@ValeurDeMarche,@Categorie1,@Categorie2,@DeviseDeCotation,@TauxObligation,@DateMaturite,@DateExpiration,@Tiers,@RAF,@BOA_SJ,@BOA_Contrepartie,@BOA_DEFAUT,@Identifiant, @Enrichi_RefCategorieRWA, @Enrichi_IdentifiantUniqueRetenu, @Enrichi_RAF, @Enrichi_LibelleOrigine, @Enrichi_DateFinContrat, @Enrichi_Commentaire, @Enrichi_Bloomberg,2, 2, @CodeResult
							
						IF (@CodeResult>300 and @CodeResult<=399)
							EXEC dbo.USP_HECATE_TRAITEMENT_SETINVENTAIRESNORMALISES @LigneCONSO,@PeriodeCloture,@Source,@Identifiant,@Nom,@ValeurDeMarche,@Categorie1,@Categorie2,@DeviseDeCotation,@TauxObligation,@DateMaturite,@DateExpiration,@Tiers,@RAF,@BOA_SJ,@BOA_Contrepartie,@BOA_DEFAUT,@Identifiant, @Enrichi_RefCategorieRWA, @Enrichi_IdentifiantUniqueRetenu, @Enrichi_RAF, @Enrichi_LibelleOrigine, @Enrichi_DateFinContrat, @Enrichi_Commentaire, @Enrichi_Bloomberg,3, 2, @CodeResult
							
						IF (@CodeResult>500 and @CodeResult<=599)
							EXEC dbo.USP_HECATE_TRAITEMENT_SETINVENTAIRESNORMALISES @LigneCONSO,@PeriodeCloture,@Source,@Identifiant,@Nom,@ValeurDeMarche,@Categorie1,@Categorie2,@DeviseDeCotation,@TauxObligation,@DateMaturite,@DateExpiration,@Tiers,@RAF,@BOA_SJ,@BOA_Contrepartie,@BOA_DEFAUT,@Identifiant, @Enrichi_RefCategorieRWA, @Enrichi_IdentifiantUniqueRetenu, @Enrichi_RAF, @Enrichi_LibelleOrigine, @Enrichi_DateFinContrat, @Enrichi_Commentaire, @Enrichi_Bloomberg,4, 2, @CodeResult
						
						SET @LigneECHEC = @LigneECHEC+1
					END
				END
				ELSE
				BEGIN
					SET @LigneEXCLU = @LigneEXCLU+1
					
					SET	@Enrichi_IdentifiantOrigine=@Identifiant
					
				EXEC TraceRwa 'CURSOR', 'WORK-EXCLU', @LigneEXCLU;
					
					EXEC dbo.USP_HECATE_TRAITEMENT_SETINVENTAIRESNORMALISES @LigneEXCLU,@PeriodeCloture,@Source,@Identifiant,@Nom,@ValeurDeMarche,@Categorie1,@Categorie2,@DeviseDeCotation,@TauxObligation,@DateMaturite,@DateExpiration,@Tiers,@RAF,@BOA_SJ,@BOA_Contrepartie,@BOA_DEFAUT,@Identifiant, @Enrichi_RefCategorieRWA, @Enrichi_IdentifiantUniqueRetenu, @Enrichi_RAF, @Enrichi_LibelleOrigine, @Enrichi_DateFinContrat, @Enrichi_Commentaire, @Enrichi_Bloomberg,1, 3, 701
					
					EXEC dbo.USP_HECATE_LOG_SETLOGCONSO @LigneEXCLU,2,@PeriodeCloture,@Source,@Identifiant,'Ligne EXCLU cause montant ValeurDeMarche.'
				END
				
				SET @NumLigne=@NumLigne+1
				
				EXEC TraceRwa 'CURSOR', 'WORK-FETCH NEXT';
				
				FETCH NEXT FROM CursorTest INTO @Identifiant, @Nom, @Categorie1, @Categorie2, @DateMaturite, @DateExpiration, @RAF, @ValeurDeMarche, @DeviseDeCotation, @TauxObligation, @Tiers, @BOA_SJ, @BOA_Contrepartie, @BOA_DEFAUT
			END
		
		CLOSE CursorTest;
		DEALLOCATE CursorTest;
		
		SET @stop = GETDATE();
		PRINT 'CONSOLIDATION--> START:' + CONVERT(nvarchar(20),@start,113) + ' STOP:' + CONVERT(nvarchar(20),@stop,113);
		
		DROP TABLE TEMP_DATA;
		
		SET @ChaineTMP = 'FIN CONSO -> TOTAL LIGNE TRAITEE('+cast(@NumLigne as nvarchar(5))+'); LIGNES CONSOLIDEE('+cast(@LigneCONSO as nvarchar(5))+') dont REUSSI('+cast(@LigneREUSSI as nvarchar(5))+') / ECHEC('+cast(@LigneECHEC as nvarchar(5))+') et LIGNES EXCLUS('+cast(@LigneEXCLU as nvarchar(5))+').'
		EXEC dbo.USP_HECATE_LOG_SETLOGIMPORTEEXPORT 0,@PeriodeCloture,@Source,'IMPORT INV NORM',0,@ChaineTMP
		PRINT @ChaineTMP
		RETURN 0;
	END
	ELSE
	BEGIN
		EXEC dbo.USP_HECATE_LOG_SETLOGIMPORTEEXPORT 2,@PeriodeCloture,@Source,'IMPORT INV NORM',1500,'Fichier non trouvé.'
		PRINT '(1500) - Fichier non trouvé.'
		RETURN 1500;
	END
END
