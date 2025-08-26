using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RWA.Web.Application.Models;

public partial class RwaContext : DbContext
{
    public RwaContext()
    {
    }

    public RwaContext(DbContextOptions<RwaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BfcCtAccountTemp> BfcCtAccountTemps { get; set; }

    public virtual DbSet<BfcCtConso1AggregTemp> BfcCtConso1AggregTemps { get; set; }

    public virtual DbSet<BfcCtConso1Temp> BfcCtConso1Temps { get; set; }

    public virtual DbSet<BfcCtConsoAggregTemp> BfcCtConsoAggregTemps { get; set; }

    public virtual DbSet<BfcCtConsoTemp> BfcCtConsoTemps { get; set; }

    public virtual DbSet<BfcCtCurncyTemp> BfcCtCurncyTemps { get; set; }

    public virtual DbSet<BfcCtEntityTemp> BfcCtEntityTemps { get; set; }

    public virtual DbSet<CommonUser> CommonUsers { get; set; }

    public virtual DbSet<HecateCatDepositaire1> HecateCatDepositaire1s { get; set; }

    public virtual DbSet<HecateCatDepositaire2> HecateCatDepositaire2s { get; set; }

    public virtual DbSet<HecateCategorieRwa> HecateCategorieRwas { get; set; }

    public virtual DbSet<HecateCompteur> HecateCompteurs { get; set; }

    public virtual DbSet<HecateCompteurTmp> HecateCompteurTmps { get; set; }

    public virtual DbSet<HecateCompteuralpha> HecateCompteuralphas { get; set; }

    public virtual DbSet<HecateContrepartiesTransparence> HecateContrepartiesTransparences { get; set; }

    public virtual DbSet<HecateCounterpartiesToSearchAndUserInput> HecateCounterpartiesToSearchAndUserInputs { get; set; }

    public virtual DbSet<HecateEquivalenceCatRwa> HecateEquivalenceCatRwas { get; set; }

    public virtual DbSet<HecateInterneHistorique> HecateInterneHistoriques { get; set; }

    public virtual DbSet<HecateInventaireNormalise> HecateInventaireNormalises { get; set; }

    public virtual DbSet<HecateLogConso> HecateLogConsos { get; set; }

    public virtual DbSet<HecateLogImportExport> HecateLogImportExports { get; set; }

    public virtual DbSet<HecateRafGeneric> HecateRafGenerics { get; set; }

    public virtual DbSet<HecateStatutTethy> HecateStatutTethys { get; set; }

    public virtual DbSet<HecateTethy> HecateTethys { get; set; }

    public virtual DbSet<HecateTypeBloomberg> HecateTypeBloombergs { get; set; }

    public virtual DbSet<HecateTypeDepot> HecateTypeDepots { get; set; }

    public virtual DbSet<HecateTypeLog> HecateTypeLogs { get; set; }

    public virtual DbSet<HecateTypeResultat> HecateTypeResultats { get; set; }

    public virtual DbSet<HeliosControlesCoherenceMontantsEncour> HeliosControlesCoherenceMontantsEncours { get; set; }

    public virtual DbSet<HeliosControlesOngletContrat> HeliosControlesOngletContrats { get; set; }

    public virtual DbSet<HeliosControlesOngletEncour> HeliosControlesOngletEncours { get; set; }

    public virtual DbSet<HeliosControlesOngletTitre> HeliosControlesOngletTitres { get; set; }

    public virtual DbSet<HeliosDatefichierTocsv> HeliosDatefichierTocsvs { get; set; }

    public virtual DbSet<HeliosImportFichierEnrichiBrut> HeliosImportFichierEnrichiBruts { get; set; }

    public virtual DbSet<HeliosImportFichierEnrichiRetraite> HeliosImportFichierEnrichiRetraites { get; set; }

    public virtual DbSet<HeliosImportFichierEnrichiRetraiteOld> HeliosImportFichierEnrichiRetraiteOlds { get; set; }

    public virtual DbSet<HeliosImportMagnitudeBrut> HeliosImportMagnitudeBruts { get; set; }

    public virtual DbSet<HeliosImportMagnitudeRetraite> HeliosImportMagnitudeRetraites { get; set; }

    public virtual DbSet<HeliosImportMagnitudeRetraiteOld> HeliosImportMagnitudeRetraiteOlds { get; set; }

    public virtual DbSet<HeliosImportSyntheseB2c> HeliosImportSyntheseB2cs { get; set; }

    public virtual DbSet<HeliosLogImportExport> HeliosLogImportExports { get; set; }

    public virtual DbSet<HeliosLogTraitement> HeliosLogTraitements { get; set; }

    public virtual DbSet<HeliosOngletContrat> HeliosOngletContrats { get; set; }

    public virtual DbSet<HeliosOngletContratsTemp> HeliosOngletContratsTemps { get; set; }

    public virtual DbSet<HeliosOngletEncour> HeliosOngletEncours { get; set; }

    public virtual DbSet<HeliosOngletEncoursPcc> HeliosOngletEncoursPccs { get; set; }

    public virtual DbSet<HeliosOngletEncoursPccTocsv> HeliosOngletEncoursPccTocsvs { get; set; }

    public virtual DbSet<HeliosOngletTitre> HeliosOngletTitres { get; set; }

    public virtual DbSet<HeliosOngletTitresTmp> HeliosOngletTitresTmps { get; set; }

    public virtual DbSet<HeliosOngletTitresTocsv> HeliosOngletTitresTocsvs { get; set; }

    public virtual DbSet<HeliosParamCategorieSyntheseAForcer> HeliosParamCategorieSyntheseAForcers { get; set; }

    public virtual DbSet<HeliosParamCategorieSyntheseAModifier> HeliosParamCategorieSyntheseAModifiers { get; set; }

    public virtual DbSet<HeliosParamClo> HeliosParamClos { get; set; }

    public virtual DbSet<HeliosParamCodesProduit> HeliosParamCodesProduits { get; set; }

    public virtual DbSet<HeliosParamComptesMagnitudeAExtraire> HeliosParamComptesMagnitudeAExtraires { get; set; }

    public virtual DbSet<HeliosParamEncoursElementaire> HeliosParamEncoursElementaires { get; set; }

    public virtual DbSet<HeliosParamEncoursElementairesPcc> HeliosParamEncoursElementairesPccs { get; set; }

    public virtual DbSet<HeliosParamEntreesPerimetre> HeliosParamEntreesPerimetres { get; set; }

    public virtual DbSet<HeliosParamIntentionGestionParCompte> HeliosParamIntentionGestionParComptes { get; set; }

    public virtual DbSet<HeliosParamMappingMagnitudeCopernicUliss> HeliosParamMappingMagnitudeCopernicUlisses { get; set; }

    public virtual DbSet<HeliosParamMappingTitresMagnitude> HeliosParamMappingTitresMagnitudes { get; set; }

    public virtual DbSet<HeliosParamMappingTitresTransparence> HeliosParamMappingTitresTransparences { get; set; }

    public virtual DbSet<HeliosParamPartnersAExclureParCompteMagnitude> HeliosParamPartnersAExclureParCompteMagnitudes { get; set; }

    public virtual DbSet<HeliosParamPerimetreDette> HeliosParamPerimetreDettes { get; set; }

    public virtual DbSet<HeliosParamRatingTitrisation> HeliosParamRatingTitrisations { get; set; }

    public virtual DbSet<HeliosPrepReportingRwaAgregee> HeliosPrepReportingRwaAgregees { get; set; }

    public virtual DbSet<HeliosPrepReportingRwaNonAgregee> HeliosPrepReportingRwaNonAgregees { get; set; }

    public virtual DbSet<HeliosPrepSyntheseRwaAgregee> HeliosPrepSyntheseRwaAgregees { get; set; }

    public virtual DbSet<HeliosPrepSyntheseRwaNonAgregee> HeliosPrepSyntheseRwaNonAgregees { get; set; }

    public virtual DbSet<HeliosPrepSyntheseRwaOld> HeliosPrepSyntheseRwaOlds { get; set; }

    public virtual DbSet<HeliosReportControle0> HeliosReportControle0s { get; set; }

    public virtual DbSet<HeliosReportControle0a> HeliosReportControle0as { get; set; }

    public virtual DbSet<HeliosReportControle0b> HeliosReportControle0bs { get; set; }

    public virtual DbSet<HeliosReportControle0c> HeliosReportControle0cs { get; set; }

    public virtual DbSet<HeliosReportControle1> HeliosReportControle1s { get; set; }

    public virtual DbSet<HeliosReportControle10> HeliosReportControle10s { get; set; }

    public virtual DbSet<HeliosReportControle2> HeliosReportControle2s { get; set; }

    public virtual DbSet<HeliosReportControle3> HeliosReportControle3s { get; set; }

    public virtual DbSet<HeliosReportControle34> HeliosReportControle34s { get; set; }

    public virtual DbSet<HeliosReportControle35> HeliosReportControle35s { get; set; }

    public virtual DbSet<HeliosReportControle36> HeliosReportControle36s { get; set; }

    public virtual DbSet<HeliosReportControle4> HeliosReportControle4s { get; set; }

    public virtual DbSet<HeliosReportControle4a> HeliosReportControle4as { get; set; }

    public virtual DbSet<HeliosReportControle6> HeliosReportControle6s { get; set; }

    public virtual DbSet<HeliosReportControle8> HeliosReportControle8s { get; set; }

    public virtual DbSet<HeliosReportControle9> HeliosReportControle9s { get; set; }

    public virtual DbSet<HeliosSeedMoney> HeliosSeedMoneys { get; set; }

    public virtual DbSet<HeliosTempExportSyntheseRwa> HeliosTempExportSyntheseRwas { get; set; }

    public virtual DbSet<HeliosTempImportMagnitudeRetraite> HeliosTempImportMagnitudeRetraites { get; set; }

    public virtual DbSet<HeliosTmpSyntheseRwaOld> HeliosTmpSyntheseRwaOlds { get; set; }

    public virtual DbSet<HeliosTypeLog> HeliosTypeLogs { get; set; }

    public virtual DbSet<RwaTrace> RwaTraces { get; set; }

    public virtual DbSet<VwHeliosOngletContratsCsv> VwHeliosOngletContratsCsvs { get; set; }

    public virtual DbSet<VwHeliosOngletEncoursCsv> VwHeliosOngletEncoursCsvs { get; set; }

    public virtual DbSet<VwHeliosOngletTitresCsv> VwHeliosOngletTitresCsvs { get; set; }

    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        // Only configure SQL Server if no options have been provided by the host (Program.cs).
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=dev-a-rwa-db-01.hc.nim-os.net;User Id=admin;Password=kangyatze;Database=RWA;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BfcCtAccountTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BFC_ct_account_temp");

            entity.Property(e => e.AccessMode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("access_mode");
            entity.Property(e => e.AuthorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("author_id");
            entity.Property(e => e.Cdesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cdesc");
            entity.Property(e => e.Class)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("class");
            entity.Property(e => e.CreationDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("creation_date");
            entity.Property(e => e.Ct0000Analys)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_analys");
            entity.Property(e => e.Ct0000Catego)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_catego");
            entity.Property(e => e.Ct0000Cfs)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_cfs");
            entity.Property(e => e.Ct0000Icdoc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_icdoc");
            entity.Property(e => e.Ct0000Parent)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_parent");
            entity.Property(e => e.Ct0000Parentbp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_parentbp");
            entity.Property(e => e.Ct0000Parentinvfs)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_parentinvfs");
            entity.Property(e => e.Ct0000Parentmyp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_parentmyp");
            entity.Property(e => e.Ct0000Parentpf)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_parentpf");
            entity.Property(e => e.Ct0000Rule1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_rule1");
            entity.Property(e => e.Ct0000Rule2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_rule2");
            entity.Property(e => e.Ct0000Rule3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_rule3");
            entity.Property(e => e.Ct0000Sbtot)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_sbtot");
            entity.Property(e => e.Ct0000Setacc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_setacc");
            entity.Property(e => e.Ct0000Setaccbp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_setaccbp");
            entity.Property(e => e.Ct0000Setaccinvfs)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_setaccinvfs");
            entity.Property(e => e.Ct0000Setaccmyp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_setaccmyp");
            entity.Property(e => e.Ct0000Setaccpf)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_setaccpf");
            entity.Property(e => e.Ct0000Test)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_test");
            entity.Property(e => e.Decimals)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("decimals");
            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Ldesc1)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("ldesc1");
            entity.Property(e => e.Ldesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc2");
            entity.Property(e => e.Ldesc3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc3");
            entity.Property(e => e.Ldesc4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc4");
            entity.Property(e => e.Ldesc5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc5");
            entity.Property(e => e.Ldesc6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc6");
            entity.Property(e => e.Monetary)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("monetary");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.OwnerSite)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("owner_site");
            entity.Property(e => e.OwnerWorkgroupId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("owner_workgroup_id");
            entity.Property(e => e.Sdesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc1");
            entity.Property(e => e.Sdesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc2");
            entity.Property(e => e.Sdesc3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc3");
            entity.Property(e => e.Sdesc4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc4");
            entity.Property(e => e.Sdesc5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc5");
            entity.Property(e => e.Sdesc6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc6");
            entity.Property(e => e.Sign)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sign");
            entity.Property(e => e.UpdateAuthorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("update_author_id");
            entity.Property(e => e.UpdateDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("update_date");
            entity.Property(e => e.Xdesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc1");
            entity.Property(e => e.Xdesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc2");
            entity.Property(e => e.Xdesc3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc3");
            entity.Property(e => e.Xdesc4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc4");
            entity.Property(e => e.Xdesc5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc5");
            entity.Property(e => e.Xdesc6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc6");
        });

        modelBuilder.Entity<BfcCtConso1AggregTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BFC_ct_conso_1_aggreg_temp");

            entity.Property(e => e.Accnt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("accnt");
            entity.Property(e => e.Consamount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("consamount");
            entity.Property(e => e.Entity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("entity");
            entity.Property(e => e.Partner)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("partner");
            entity.Property(e => e.Period)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("period");
        });

        modelBuilder.Entity<BfcCtConso1Temp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BFC_ct_conso_1_temp");

            entity.Property(e => e.Accnt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("accnt");
            entity.Property(e => e.Amount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("amount");
            entity.Property(e => e.Consamount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("consamount");
            entity.Property(e => e.Convamount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("convamount");
            entity.Property(e => e.Ct0000Al)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_al");
            entity.Property(e => e.Ct0000At)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_at");
            entity.Property(e => e.Ct0000At2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_at2");
            entity.Property(e => e.Ct0000Ct)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_ct");
            entity.Property(e => e.Ct0000Ga)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_ga");
            entity.Property(e => e.Ct0000Pt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_pt");
            entity.Property(e => e.Ct0000Tc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_tc");
            entity.Property(e => e.Ctshare)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ctshare");
            entity.Property(e => e.Curncy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("curncy");
            entity.Property(e => e.DataComment)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("data_comment");
            entity.Property(e => e.Entity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("entity");
            entity.Property(e => e.Entorig)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("entorig");
            entity.Property(e => e.Enumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("enumber");
            entity.Property(e => e.Flow)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("flow");
            entity.Property(e => e.Globorig)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("globorig");
            entity.Property(e => e.Journal)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("journal");
            entity.Property(e => e.Mu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mu");
            entity.Property(e => e.Nature)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nature");
            entity.Property(e => e.Partner)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("partner");
            entity.Property(e => e.Period)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("period");
            entity.Property(e => e.Pmu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pmu");
            entity.Property(e => e.Rollup)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rollup");
            entity.Property(e => e.Rollup2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rollup2");
            entity.Property(e => e.RollupPartner)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rollup_partner");
            entity.Property(e => e.RollupPartner2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rollup_partner2");
            entity.Property(e => e.Techorig)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("techorig");
        });

        modelBuilder.Entity<BfcCtConsoAggregTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BFC_ct_conso_aggreg_temp");

            entity.Property(e => e.Accnt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("accnt");
            entity.Property(e => e.Consamount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("consamount");
            entity.Property(e => e.Entity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("entity");
            entity.Property(e => e.Period)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("period");
        });

        modelBuilder.Entity<BfcCtConsoTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BFC_ct_conso_temp");

            entity.Property(e => e.Accnt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("accnt");
            entity.Property(e => e.Amount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("amount");
            entity.Property(e => e.Consamount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("consamount");
            entity.Property(e => e.Convamount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("convamount");
            entity.Property(e => e.Ct0000Al)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_al");
            entity.Property(e => e.Ct0000At)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_at");
            entity.Property(e => e.Ct0000At2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_at2");
            entity.Property(e => e.Ct0000Ct)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_ct");
            entity.Property(e => e.Ct0000Ga)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_ga");
            entity.Property(e => e.Ct0000Pt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_pt");
            entity.Property(e => e.Ct0000Tc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_tc");
            entity.Property(e => e.Ctshare)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ctshare");
            entity.Property(e => e.Curncy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("curncy");
            entity.Property(e => e.DataComment)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("data_comment");
            entity.Property(e => e.Entity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("entity");
            entity.Property(e => e.Entorig)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("entorig");
            entity.Property(e => e.Enumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("enumber");
            entity.Property(e => e.Flow)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("flow");
            entity.Property(e => e.Globorig)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("globorig");
            entity.Property(e => e.Journal)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("journal");
            entity.Property(e => e.Mu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mu");
            entity.Property(e => e.Nature)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nature");
            entity.Property(e => e.Partner)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("partner");
            entity.Property(e => e.Period)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("period");
            entity.Property(e => e.Pmu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pmu");
            entity.Property(e => e.Rollup)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rollup");
            entity.Property(e => e.Rollup2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rollup2");
            entity.Property(e => e.RollupPartner)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rollup_partner");
            entity.Property(e => e.RollupPartner2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rollup_partner2");
            entity.Property(e => e.Techorig)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("techorig");
        });

        modelBuilder.Entity<BfcCtCurncyTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BFC_ct_curncy_temp");

            entity.Property(e => e.AccessMode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("access_mode");
            entity.Property(e => e.AuthorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("author_id");
            entity.Property(e => e.Cdesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cdesc");
            entity.Property(e => e.CreationDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("creation_date");
            entity.Property(e => e.Ct0000Espcur)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_espcur");
            entity.Property(e => e.Decimals)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("decimals");
            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Ldesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc1");
            entity.Property(e => e.Ldesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc2");
            entity.Property(e => e.Ldesc3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc3");
            entity.Property(e => e.Ldesc4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc4");
            entity.Property(e => e.Ldesc5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc5");
            entity.Property(e => e.Ldesc6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc6");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.OwnerSite)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("owner_site");
            entity.Property(e => e.OwnerWorkgroupId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("owner_workgroup_id");
            entity.Property(e => e.Sdesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc1");
            entity.Property(e => e.Sdesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc2");
            entity.Property(e => e.Sdesc3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc3");
            entity.Property(e => e.Sdesc4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc4");
            entity.Property(e => e.Sdesc5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc5");
            entity.Property(e => e.Sdesc6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc6");
            entity.Property(e => e.UpdateAuthorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("update_author_id");
            entity.Property(e => e.UpdateDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("update_date");
            entity.Property(e => e.Xdesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc1");
            entity.Property(e => e.Xdesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc2");
            entity.Property(e => e.Xdesc3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc3");
            entity.Property(e => e.Xdesc4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc4");
            entity.Property(e => e.Xdesc5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc5");
            entity.Property(e => e.Xdesc6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc6");
        });

        modelBuilder.Entity<BfcCtEntityTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BFC_ct_entity_temp");

            entity.Property(e => e.AccessMode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("access_mode");
            entity.Property(e => e.AuthorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("author_id");
            entity.Property(e => e.Cdesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cdesc");
            entity.Property(e => e.Company)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("company");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("country");
            entity.Property(e => e.CreationDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("creation_date");
            entity.Property(e => e.Ct0000Adress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_adress");
            entity.Property(e => e.Ct0000Area)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_area");
            entity.Property(e => e.Ct0000At)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_at");
            entity.Property(e => e.Ct0000Boa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_boa");
            entity.Property(e => e.Ct0000Contact)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_contact");
            entity.Property(e => e.Ct0000Etype)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_etype");
            entity.Property(e => e.Ct0000Objective)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_objective");
            entity.Property(e => e.Ct0000Tel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_tel");
            entity.Property(e => e.Ct0000Zone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ct_0000_zone");
            entity.Property(e => e.Curncy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("curncy");
            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Ldesc1)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("ldesc1");
            entity.Property(e => e.Ldesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc2");
            entity.Property(e => e.Ldesc3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc3");
            entity.Property(e => e.Ldesc4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc4");
            entity.Property(e => e.Ldesc5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc5");
            entity.Property(e => e.Ldesc6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ldesc6");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.OwnerSite)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("owner_site");
            entity.Property(e => e.OwnerWorkgroupId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("owner_workgroup_id");
            entity.Property(e => e.Sdesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc1");
            entity.Property(e => e.Sdesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc2");
            entity.Property(e => e.Sdesc3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc3");
            entity.Property(e => e.Sdesc4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc4");
            entity.Property(e => e.Sdesc5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc5");
            entity.Property(e => e.Sdesc6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sdesc6");
            entity.Property(e => e.UpdateAuthorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("update_author_id");
            entity.Property(e => e.UpdateDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("update_date");
            entity.Property(e => e.Xdesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc1");
            entity.Property(e => e.Xdesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc2");
            entity.Property(e => e.Xdesc3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc3");
            entity.Property(e => e.Xdesc4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc4");
            entity.Property(e => e.Xdesc5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc5");
            entity.Property(e => e.Xdesc6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("xdesc6");
        });

        modelBuilder.Entity<CommonUser>(entity =>
        {
            entity.HasKey(e => e.Userid);

            entity.ToTable("COMMON_USER");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Emailaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("emailaddress");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Login)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(10)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<HecateCatDepositaire1>(entity =>
        {
            entity.HasKey(e => e.IdDepositaire1).HasName("PK_TBL_CAT_DEPOSITAIRE1");

            entity.ToTable("HECATE_CAT_DEPOSITAIRE1");

            entity.HasIndex(e => e.LibelleDepositaire1, "IX_TBL_CAT_DEPOSITAIRE1").IsUnique();

            entity.Property(e => e.LibelleDepositaire1).HasMaxLength(100);
        });

        modelBuilder.Entity<HecateCatDepositaire2>(entity =>
        {
            entity.HasKey(e => e.IdDepositaire2).HasName("PK_TBL_CAT_DEPOSITAIRE2");

            entity.ToTable("HECATE_CAT_DEPOSITAIRE2");

            entity.HasIndex(e => e.LibelleDepositaire2, "IX_TBL_CAT_DEPOSITAIRE2").IsUnique();

            entity.Property(e => e.LibelleDepositaire2).HasMaxLength(100);
        });

        modelBuilder.Entity<HecateCategorieRwa>(entity =>
        {
            entity.HasKey(e => e.IdCatRwa).HasName("PK_TBL_CATEGORIE_RWA");

            entity.ToTable("HECATE_CATEGORIE_RWA");

            entity.Property(e => e.IdCatRwa)
                .HasMaxLength(3)
                .HasColumnName("IdCatRWA");
            entity.Property(e => e.Libelle).HasMaxLength(50);
            entity.Property(e => e.ValeurMobiliere)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<HecateCompteur>(entity =>
        {
            entity.HasKey(e => new { e.Source, e.RefCategorieRwa, e.PeriodeCloture });

            entity.ToTable("HECATE_COMPTEUR");

            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
        });

        modelBuilder.Entity<HecateCompteurTmp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HECATE_COMPTEUR_TMP");

            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HecateCompteuralpha>(entity =>
        {
            entity.HasKey(e => e.Idcompteuralpha);

            entity.ToTable("HECATE_COMPTEURALPHA");

            entity.Property(e => e.Compteuralpha)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<HecateContrepartiesTransparence>(entity =>
        {
            entity.HasKey(e => e.LibelleContrepartieOrigine);

            entity
                .ToTable("HECATE_CONTREPARTIES_TRANSPARENCE");

            entity.Property(e => e.LibelleContrepartieOrigine)
                .HasMaxLength(255)
                .HasColumnName("LIBELLE CONTREPARTIE ORIGINE");
            entity.Property(e => e.LibelleCourtTethys)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("LIBELLE COURT TETHYS");
            entity.Property(e => e.LibelleGroupeTethys)
                .HasMaxLength(71)
                .IsUnicode(false)
                .HasColumnName("LIBELLE GROUPE TETHYS");
            entity.Property(e => e.RafEntite)
                .HasMaxLength(7)
                .HasColumnName("RAF ENTITE");
            entity.Property(e => e.RafGroupe)
                .HasMaxLength(7)
                .HasColumnName("RAF GROUPE");
            entity.Property(e => e.RaisonSocialeTethys)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("RAISON SOCIALE TETHYS");
        });

        modelBuilder.Entity<HecateCounterpartiesToSearchAndUserInput>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HECATE_COUNTERPARTIES_TO_SEARCH_AND_USER_INPUT");

            entity.Property(e => e.Counterparty)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.UserSearch1).HasMaxLength(200);
            entity.Property(e => e.UserSearch2).HasMaxLength(200);
            entity.Property(e => e.UserSearch3).HasMaxLength(200);
        });

        modelBuilder.Entity<HecateEquivalenceCatRwa>(entity =>
        {
            entity.HasKey(e => new { e.Source, e.RefCatDepositaire1, e.RefCatDepositaire2 }).HasName("PK_TBL_EQUIVALENCE_CAT_RWA");

            entity.ToTable("HECATE_EQUIVALENCE_CAT_RWA");

            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.RefTypeBloomberg).HasMaxLength(10);

            entity.HasOne(d => d.RefCatDepositaire1Navigation).WithMany(p => p.HecateEquivalenceCatRwas)
                .HasForeignKey(d => d.RefCatDepositaire1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TBL_EQUIVALENCE_CAT_RWA_TBL_CAT_DEPOSITAIRE1");

            entity.HasOne(d => d.RefCatDepositaire2Navigation).WithMany(p => p.HecateEquivalenceCatRwas)
                .HasForeignKey(d => d.RefCatDepositaire2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TBL_EQUIVALENCE_CAT_RWA_TBL_CAT_DEPOSITAIRE2");

            entity.HasOne(d => d.RefCategorieRwaNavigation).WithMany(p => p.HecateEquivalenceCatRwas)
                .HasForeignKey(d => d.RefCategorieRwa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TBL_EQUIVALENCE_CAT_RWA_TBL_CATEGORIE_RWA1");

            entity.HasOne(d => d.RefTypeBloombergNavigation).WithMany(p => p.HecateEquivalenceCatRwas)
                .HasForeignKey(d => d.RefTypeBloomberg)
                .HasConstraintName("FK_TBL_EQUIVALENCE_CAT_RWA_TBL_TYPE_BLOOMBERG");
        });

        modelBuilder.Entity<HecateInterneHistorique>(entity =>
        {
            entity.HasKey(e => new { e.Source, e.IdentifiantOrigine, e.LibelleOrigine }).HasName("PK_BDD_INTERNE_HISTORIQUE");

            entity.ToTable("HECATE_INTERNE_HISTORIQUE");

            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.IdentifiantOrigine).HasMaxLength(50);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.Bbgticker)
                .HasMaxLength(30)
                .HasColumnName("BBGTicker");
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.LibelleTypeDette).HasMaxLength(12);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
        });

        modelBuilder.Entity<HecateInventaireNormalise>(entity =>
        {
            // Use NumLigne as the primary key so EF can track imported rows.
            // NumLigne is assigned by the import process and is NOT NULL in the table DDL.
            entity.HasKey(e => e.NumLigne);
            entity.ToTable("HECATE_INVENTAIRE_NORMALISES");

            entity.Property(e => e.Bloomberg).HasMaxLength(70);
            entity.Property(e => e.BoaContrepartie)
                .HasMaxLength(7)
                .HasColumnName("BOA_Contrepartie");
            entity.Property(e => e.BoaDefaut)
                .HasMaxLength(7)
                .HasColumnName("BOA_DEFAUT");
            entity.Property(e => e.BoaSj)
                .HasMaxLength(7)
                .HasColumnName("BOA_SJ");
            entity.Property(e => e.Categorie1).HasMaxLength(100);
            entity.Property(e => e.Categorie2).HasMaxLength(100);
            entity.Property(e => e.Commentaires).HasMaxLength(200);
            entity.Property(e => e.DeviseDeCotation).HasMaxLength(50);
            entity.Property(e => e.Identifiant).HasMaxLength(50);
            entity.Property(e => e.IdentifiantOrigine).HasMaxLength(50);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.Nom).HasMaxLength(255);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.Rafenrichi)
                .HasMaxLength(7)
                .HasColumnName("RAFEnrichi");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.TauxObligation).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Tiers).HasMaxLength(255);

            entity.HasOne(d => d.RefTypeDepotNavigation).WithMany()
                .HasForeignKey(d => d.RefTypeDepot)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HECATE_INVENTAIRE_NORMALISES_HECATE_TYPE_DEPOT");

            entity.HasOne(d => d.RefTypeResultatNavigation).WithMany()
                .HasForeignKey(d => d.RefTypeResultat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HECATE_INVENTAIRE_NORMALISES_HECATE_TYPE_RESULTAT");
        });

        modelBuilder.Entity<HecateLogConso>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HECATE_LOG_CONSO");

            entity.Property(e => e.IdentifiantOrigine).HasMaxLength(50);
            entity.Property(e => e.LogTime).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Source).HasMaxLength(3);

            entity.HasOne(d => d.RefTypeLogNavigation).WithMany()
                .HasForeignKey(d => d.RefTypeLog)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HECATE_LOG_HECATE_TYPE_LOG");
        });

        modelBuilder.Entity<HecateLogImportExport>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HECATE_LOG_IMPORT_EXPORT");

            entity.Property(e => e.LogTime).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Process).HasMaxLength(50);
            entity.Property(e => e.Source).HasMaxLength(3);

            entity.HasOne(d => d.RefTypeLogNavigation).WithMany()
                .HasForeignKey(d => d.RefTypeLog)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HECATE_LOG_IMPORT_EXPORT_HECATE_TYPE_LOG");
        });

        modelBuilder.Entity<HecateRafGeneric>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HECATE_RAF_GENERIC");

            entity.Property(e => e.RafGeneric).HasMaxLength(50);
            entity.Property(e => e.RafGenericName).HasMaxLength(50);
        });

        modelBuilder.Entity<HecateStatutTethy>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HECATE_STATUT_TETHYS");

            entity.Property(e => e.Counterparty)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Raf)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.StatutTethys)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<HecateTethy>(entity =>
        {
            entity.HasKey(e => new { e.IdentifiantRaf, e.CodeIsin, e.CodeCusip });

            entity.ToTable("HECATE_TETHYS");

            entity.HasIndex(e => e.CodeIsin, "IX1_HECATE_TETHYS");

            entity.HasIndex(e => e.CodeCusip, "IX2_HECATE_TETHYS");

            entity.Property(e => e.CategorieTethys)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("CATEGORIE TETHYS");
            entity.Property(e => e.CodeApparentement)
                .HasMaxLength(18)
                .IsUnicode(false)
                .HasColumnName("CODE APPARENTEMENT");
            entity.Property(e => e.CodeConso)
                .HasMaxLength(31)
                .IsUnicode(false)
                .HasColumnName("CODE CONSO");
            entity.Property(e => e.CodeCusip)
                .HasMaxLength(31)
                .IsUnicode(false)
                .HasColumnName("CODE CUSIP");
            entity.Property(e => e.CodeIsin)
                .HasMaxLength(31)
                .IsUnicode(false)
                .HasColumnName("CODE ISIN");
            entity.Property(e => e.CodeNotation)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("CODE NOTATION");
            entity.Property(e => e.DateNotationInterne)
                .HasMaxLength(21)
                .IsUnicode(false)
                .HasColumnName("DATE NOTATION INTERNE");
            entity.Property(e => e.IdentifiantRaf)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDENTIFIANT RAF");
            entity.Property(e => e.LibelleCourt)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("LIBELLE COURT");
            entity.Property(e => e.NafNace)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("NAF NACE");
            entity.Property(e => e.NomTeteGroupeReglementaire)
                .HasMaxLength(71)
                .IsUnicode(false)
                .HasColumnName("NOM TETE GROUPE REGLEMENTAIRE");
            entity.Property(e => e.NumeroEtNomDeRue)
                .HasMaxLength(51)
                .IsUnicode(false)
                .HasColumnName("NUMERO ET NOM DE RUE");
            entity.Property(e => e.PaysDeNationalite)
                .HasMaxLength(19)
                .IsUnicode(false)
                .HasColumnName("PAYS DE NATIONALITE");
            entity.Property(e => e.PaysDeResidence)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("PAYS DE RESIDENCE");
            entity.Property(e => e.RafTeteGroupeReglementaire)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("RAF TETE GROUPE REGLEMENTAIRE");
            entity.Property(e => e.RaisonSociale)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("RAISON SOCIALE");
            entity.Property(e => e.SegmentDeRisque)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("SEGMENT DE RISQUE");
            entity.Property(e => e.SegmentationBpce)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("SEGMENTATION BPCE");
            entity.Property(e => e.Ville)
                .HasMaxLength(51)
                .IsUnicode(false)
                .HasColumnName("VILLE");
        });

        modelBuilder.Entity<HecateTypeBloomberg>(entity =>
        {
            entity.HasKey(e => e.IdTypeBloomberg).HasName("PK_TBL_TYPE_BLOOMBERG");

            entity.ToTable("HECATE_TYPE_BLOOMBERG");

            entity.HasIndex(e => e.Libelle, "IX_HECATE_TYPE_BLOOMBERG").IsUnique();

            entity.Property(e => e.IdTypeBloomberg).HasMaxLength(10);
            entity.Property(e => e.Libelle).HasMaxLength(50);
        });

        modelBuilder.Entity<HecateTypeDepot>(entity =>
        {
            entity.HasKey(e => e.IdTypeDepo);

            entity.ToTable("HECATE_TYPE_DEPOT");

            entity.HasIndex(e => e.Name, "IX_HECATE_TYPE_DEPOT").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<HecateTypeLog>(entity =>
        {
            entity.HasKey(e => e.IdTypeLog);

            entity.ToTable("HECATE_TYPE_LOG");

            entity.HasIndex(e => e.Type, "IX_HECATE_TYPE_LOG").IsUnique();

            entity.Property(e => e.Type).HasMaxLength(20);
        });

        modelBuilder.Entity<HecateTypeResultat>(entity =>
        {
            entity.HasKey(e => e.IdTypeResultat);

            entity.ToTable("HECATE_TYPE_RESULTAT");

            entity.HasIndex(e => e.Resultat, "IX_HECATE_TYPE_RESULTAT").IsUnique();

            entity.Property(e => e.Resultat).HasMaxLength(6);
        });

        modelBuilder.Entity<HeliosControlesCoherenceMontantsEncour>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_CONTROLES_COHERENCE_MONTANTS_ENCOURS");

            entity.Property(e => e.Commentaires)
                .HasMaxLength(57)
                .IsUnicode(false);
            entity.Property(e => e.Compte).HasMaxLength(7);
            entity.Property(e => e.Source).HasMaxLength(10);
        });

        modelBuilder.Entity<HeliosControlesOngletContrat>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_CONTROLES_ONGLET_CONTRATS");

            entity.Property(e => e.ControleCodeActivite).HasMaxLength(107);
            entity.Property(e => e.ControleCodeSegmentMcDonough).HasMaxLength(75);
            entity.Property(e => e.ControleDateInitialeFinContrat).HasMaxLength(69);
            entity.Property(e => e.ControleDevise).HasMaxLength(39);
            entity.Property(e => e.ControleDoublon)
                .HasMaxLength(38)
                .IsUnicode(false);
            entity.Property(e => e.ControlePerimetreMcDonough).HasMaxLength(98);
            entity.Property(e => e.ControlePresenceEncours)
                .HasMaxLength(69)
                .IsUnicode(false);
            entity.Property(e => e.ControlePresenceTitres)
                .HasMaxLength(65)
                .IsUnicode(false);
            entity.Property(e => e.ControleRaf)
                .HasMaxLength(58)
                .HasColumnName("ControleRAF");
            entity.Property(e => e.IdentifiantContrat).HasMaxLength(30);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(8);
        });

        modelBuilder.Entity<HeliosControlesOngletEncour>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_CONTROLES_ONGLET_ENCOURS");

            entity.Property(e => e.ControleEncoursProvisionnels).HasMaxLength(83);
            entity.Property(e => e.ControlePresenceContrats)
                .HasMaxLength(70)
                .IsUnicode(false);
            entity.Property(e => e.ControleRaf)
                .HasMaxLength(62)
                .HasColumnName("ControleRAF");
            entity.Property(e => e.IdentifiantContrat).HasMaxLength(16);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(8);
        });

        modelBuilder.Entity<HeliosControlesOngletTitre>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_CONTROLES_ONGLET_TITRES");

            entity.Property(e => e.ControleCodeDeviseNominal).HasMaxLength(70);
            entity.Property(e => e.ControleDoublon)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ControleIdentifiantTitre).HasMaxLength(64);
            entity.Property(e => e.ControlePresenceContrats)
                .HasMaxLength(68)
                .IsUnicode(false);
            entity.Property(e => e.ControleRaf)
                .HasMaxLength(61)
                .HasColumnName("ControleRAF");
            entity.Property(e => e.ControleTitreCote).HasMaxLength(141);
            entity.Property(e => e.ControleTitreNote).HasMaxLength(127);
            entity.Property(e => e.ControleTypeTitre).HasMaxLength(67);
            entity.Property(e => e.IdentifiantTitre).HasMaxLength(16);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(8);
        });

        modelBuilder.Entity<HeliosDatefichierTocsv>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_DATEFICHIER_TOCSV");

            entity.Property(e => e.Dd)
                .HasMaxLength(2)
                .HasColumnName("DD");
            entity.Property(e => e.Mm)
                .HasMaxLength(2)
                .HasColumnName("MM");
            entity.Property(e => e.Yyyy)
                .HasMaxLength(4)
                .HasColumnName("YYYY");
        });

        modelBuilder.Entity<HeliosImportFichierEnrichiBrut>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_IMPORT_FICHIER_ENRICHI_BRUT");

            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosImportFichierEnrichiRetraite>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_IMPORT_FICHIER_ENRICHI_RETRAITE");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosImportFichierEnrichiRetraiteOld>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_IMPORT_FICHIER_ENRICHI_RETRAITE_OLD");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosImportMagnitudeBrut>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_IMPORT_MAGNITUDE_BRUT");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibellePartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibellePartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.ReportingUnit).HasMaxLength(5);
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosImportMagnitudeRetraite>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_IMPORT_MAGNITUDE_RETRAITE");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibellePartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibellePartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosImportMagnitudeRetraiteOld>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_IMPORT_MAGNITUDE_RETRAITE_OLD");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibellePartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibellePartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosImportSyntheseB2c>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_IMPORT_SYNTHESE_B2C");

            entity.Property(e => e.Approche).HasMaxLength(255);
            entity.Property(e => e.CategorieProduit).HasMaxLength(255);
            entity.Property(e => e.ClasseActifB2).HasMaxLength(255);
            entity.Property(e => e.ExternalId)
                .HasMaxLength(255)
                .HasColumnName("ExternalID");
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Rafb2c)
                .HasMaxLength(7)
                .HasColumnName("RAFB2C");
            entity.Property(e => e.Rating).HasMaxLength(255);
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosLogImportExport>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_LOG_IMPORT_EXPORT");

            entity.Property(e => e.LogTime).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Process).HasMaxLength(50);
            entity.Property(e => e.Source).HasMaxLength(3);

            entity.HasOne(d => d.RefTypeLogNavigation).WithMany()
                .HasForeignKey(d => d.RefTypeLog)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HELIOS_LOG_IMPORT_EXPORT_HELIOS_TYPE_LOG");
        });

        modelBuilder.Entity<HeliosLogTraitement>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_LOG_TRAITEMENTS");

            entity.Property(e => e.LogTime).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Process).HasMaxLength(50);
            entity.Property(e => e.Source).HasMaxLength(3);

            entity.HasOne(d => d.RefTypeLogNavigation).WithMany()
                .HasForeignKey(d => d.RefTypeLog)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HELIOS_LOG_TRAITEMENTS_HELIOS_TYPE_LOG");
        });

        modelBuilder.Entity<HeliosOngletContrat>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_ONGLET_CONTRATS");

            entity.Property(e => e.ClasseLgd)
                .HasMaxLength(255)
                .HasColumnName("ClasseLGD");
            entity.Property(e => e.CodeActivite).HasMaxLength(50);
            entity.Property(e => e.CodeEtablissement).HasMaxLength(50);
            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CodeSegmentMcDo).HasMaxLength(2);
            entity.Property(e => e.CodeUfonegociatrice)
                .HasMaxLength(50)
                .HasColumnName("CodeUFONegociatrice");
            entity.Property(e => e.DateDebutFinancement).HasMaxLength(10);
            entity.Property(e => e.DateEffet).HasMaxLength(10);
            entity.Property(e => e.DateFinContrat).HasMaxLength(10);
            entity.Property(e => e.DateInitialeFinContrat).HasMaxLength(10);
            entity.Property(e => e.DateSignatureContrat).HasMaxLength(10);
            entity.Property(e => e.Devise).HasMaxLength(3);
            entity.Property(e => e.Diversifiee).HasMaxLength(1);
            entity.Property(e => e.Fcec)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("FCEC");
            entity.Property(e => e.FlagComptesSegregues).HasMaxLength(1);
            entity.Property(e => e.IdentifiantContrat).HasMaxLength(30);
            entity.Property(e => e.IdentifiantIsin)
                .HasMaxLength(16)
                .HasColumnName("IdentifiantISIN");
            entity.Property(e => e.IndicateurPerimetreMcDo).HasMaxLength(1);
            entity.Property(e => e.LastUpdate)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.LibelleTypeDette).HasMaxLength(15);
            entity.Property(e => e.ManagementIntent).HasMaxLength(2);
            entity.Property(e => e.MethodeBaleIi)
                .HasMaxLength(7)
                .HasColumnName("MethodeBaleII");
            entity.Property(e => e.MotifExclusionMcDo).HasMaxLength(2);
            entity.Property(e => e.NbEncoursElementaires).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.NombreGaranties).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.PaysRisque).HasMaxLength(2);
            entity.Property(e => e.PortefeuilleIas)
                .HasMaxLength(3)
                .HasColumnName("PortefeuilleIAS");
            entity.Property(e => e.PourcentageLoanToValueActualized).HasMaxLength(3);
            entity.Property(e => e.PourcentageLoanToValueCrr3)
                .HasMaxLength(3)
                .HasColumnName("PourcentageLoanToValueCRR3");
            entity.Property(e => e.PourcentageLoanToValueOrigination).HasMaxLength(3);
            entity.Property(e => e.PresenceCreditPartage).HasMaxLength(1);
            entity.Property(e => e.PresenceGarantie).HasMaxLength(50);
            entity.Property(e => e.PrixExerciceOption).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.PrixNegocie).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.PrixUnitaireContrat).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.ProduitAs400)
                .HasMaxLength(6)
                .HasColumnName("ProduitAS400");
            entity.Property(e => e.PtfReglementaireMcDo).HasMaxLength(7);
            entity.Property(e => e.Quantite).HasMaxLength(17);
            entity.Property(e => e.Raftiers)
                .HasMaxLength(7)
                .HasColumnName("RAFTiers");
            entity.Property(e => e.ReferenceContratNettingReglem).HasMaxLength(30);
            entity.Property(e => e.ReferenceMontage).HasMaxLength(8);
            entity.Property(e => e.Section).HasMaxLength(2);
            entity.Property(e => e.SensOperation).HasMaxLength(1);
            entity.Property(e => e.StrategieFrench).HasMaxLength(3);
            entity.Property(e => e.TauxLgd)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("TauxLGD");
            entity.Property(e => e.TopTransparence).HasMaxLength(50);
            entity.Property(e => e.TypeContratNetting).HasMaxLength(1);
            entity.Property(e => e.TypeOption).HasMaxLength(2);
            entity.Property(e => e.TypeParticipation).HasMaxLength(1);
        });

        modelBuilder.Entity<HeliosOngletContratsTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_ONGLET_CONTRATS_TEMP");

            entity.Property(e => e.ClasseLgd)
                .HasMaxLength(255)
                .HasColumnName("ClasseLGD");
            entity.Property(e => e.CodeActivite).HasMaxLength(50);
            entity.Property(e => e.CodeEtablissement).HasMaxLength(50);
            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CodeSegmentMcDo).HasMaxLength(2);
            entity.Property(e => e.CodeUfonegociatrice)
                .HasMaxLength(50)
                .HasColumnName("CodeUFONegociatrice");
            entity.Property(e => e.DateDebutFinancement).HasMaxLength(10);
            entity.Property(e => e.DateEffet).HasMaxLength(10);
            entity.Property(e => e.DateFinContrat).HasMaxLength(10);
            entity.Property(e => e.DateInitialeFinContrat).HasMaxLength(10);
            entity.Property(e => e.DateSignatureContrat).HasMaxLength(10);
            entity.Property(e => e.Devise).HasMaxLength(3);
            entity.Property(e => e.Diversifiee).HasMaxLength(1);
            entity.Property(e => e.Fcec)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("FCEC");
            entity.Property(e => e.FlagComptesSegregues).HasMaxLength(1);
            entity.Property(e => e.IdentifiantContrat).HasMaxLength(30);
            entity.Property(e => e.IdentifiantIsin)
                .HasMaxLength(16)
                .HasColumnName("IdentifiantISIN");
            entity.Property(e => e.IndicateurPerimetreMcDo).HasMaxLength(1);
            entity.Property(e => e.LastUpdate)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.LibelleTypeDette).HasMaxLength(15);
            entity.Property(e => e.ManagementIntent).HasMaxLength(2);
            entity.Property(e => e.MethodeBaleIi)
                .HasMaxLength(7)
                .HasColumnName("MethodeBaleII");
            entity.Property(e => e.MotifExclusionMcDo).HasMaxLength(2);
            entity.Property(e => e.NbEncoursElementaires).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.NombreGaranties).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.PaysRisque).HasMaxLength(2);
            entity.Property(e => e.PortefeuilleIas)
                .HasMaxLength(3)
                .HasColumnName("PortefeuilleIAS");
            entity.Property(e => e.PourcentageLoanToValueActualized).HasMaxLength(3);
            entity.Property(e => e.PourcentageLoanToValueCrr3)
                .HasMaxLength(3)
                .HasColumnName("PourcentageLoanToValueCRR3");
            entity.Property(e => e.PourcentageLoanToValueOrigination).HasMaxLength(3);
            entity.Property(e => e.PresenceCreditPartage).HasMaxLength(1);
            entity.Property(e => e.PresenceGarantie).HasMaxLength(50);
            entity.Property(e => e.PrixExerciceOption).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.PrixNegocie).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.PrixUnitaireContrat).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.ProduitAs400)
                .HasMaxLength(6)
                .HasColumnName("ProduitAS400");
            entity.Property(e => e.PtfReglementaireMcDo).HasMaxLength(7);
            entity.Property(e => e.Quantite).HasMaxLength(17);
            entity.Property(e => e.Raftiers)
                .HasMaxLength(7)
                .HasColumnName("RAFTiers");
            entity.Property(e => e.ReferenceContratNettingReglem).HasMaxLength(30);
            entity.Property(e => e.ReferenceMontage).HasMaxLength(8);
            entity.Property(e => e.Section).HasMaxLength(2);
            entity.Property(e => e.SensOperation).HasMaxLength(1);
            entity.Property(e => e.StrategieFrench).HasMaxLength(3);
            entity.Property(e => e.TauxLgd)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("TauxLGD");
            entity.Property(e => e.TopTransparence).HasMaxLength(50);
            entity.Property(e => e.TypeContratNetting).HasMaxLength(1);
            entity.Property(e => e.TypeOption).HasMaxLength(2);
            entity.Property(e => e.TypeParticipation).HasMaxLength(1);
        });

        modelBuilder.Entity<HeliosOngletEncour>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_ONGLET_ENCOURS");

            entity.Property(e => e.CodeEtablissement).HasMaxLength(50);
            entity.Property(e => e.CodeUfonegociatrice)
                .HasMaxLength(50)
                .HasColumnName("CodeUFONegociatrice");
            entity.Property(e => e.DateDebutImpact).HasMaxLength(10);
            entity.Property(e => e.DateFinImpact).HasMaxLength(10);
            entity.Property(e => e.DeviseEncours).HasMaxLength(50);
            entity.Property(e => e.Elcredit)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("ELCredit");
            entity.Property(e => e.Eldilution)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("ELDilution");
            entity.Property(e => e.FixiteTauxContrat).HasMaxLength(2);
            entity.Property(e => e.FlagEncoursMcDo).HasMaxLength(50);
            entity.Property(e => e.IdentifiantContrat).HasMaxLength(30);
            entity.Property(e => e.MontantEncours).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.NumTirage).HasMaxLength(50);
            entity.Property(e => e.NumeroEncours).HasMaxLength(4);
            entity.Property(e => e.Pcecencours)
                .HasMaxLength(8)
                .HasColumnName("PCECEncours");
            entity.Property(e => e.PeriodiciteInterets).HasMaxLength(2);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.TypeEncours).HasMaxLength(5);
            entity.Property(e => e.ValeurTauxFixe).HasColumnType("numeric(15, 0)");
        });

        modelBuilder.Entity<HeliosOngletEncoursPcc>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_ONGLET_ENCOURS_PCC");

            entity.Property(e => e.CodeEtablissement).HasMaxLength(50);
            entity.Property(e => e.CodeUfonegociatrice)
                .HasMaxLength(50)
                .HasColumnName("CodeUFONegociatrice");
            entity.Property(e => e.DateDebutImpact).HasMaxLength(10);
            entity.Property(e => e.DateFinImpact).HasMaxLength(10);
            entity.Property(e => e.DeviseEncours).HasMaxLength(50);
            entity.Property(e => e.Elcredit)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("ELCredit");
            entity.Property(e => e.Eldilution)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("ELDilution");
            entity.Property(e => e.FixiteTauxContrat).HasMaxLength(2);
            entity.Property(e => e.FlagEncoursMcDo).HasMaxLength(50);
            entity.Property(e => e.IdentifiantContrat).HasMaxLength(30);
            entity.Property(e => e.MontantEncours).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.NumTirage).HasMaxLength(50);
            entity.Property(e => e.NumeroEncours).HasMaxLength(4);
            entity.Property(e => e.Pccencours)
                .HasMaxLength(9)
                .HasColumnName("PCCEncours");
            entity.Property(e => e.PeriodiciteInterets).HasMaxLength(2);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.TypeEncours).HasMaxLength(5);
            entity.Property(e => e.ValeurTauxFixe).HasColumnType("numeric(15, 0)");
        });

        modelBuilder.Entity<HeliosOngletEncoursPccTocsv>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_ONGLET_ENCOURS_PCC_TOCSV");

            entity.Property(e => e.CodeEtablissement).HasMaxLength(50);
            entity.Property(e => e.CodeUfonegociatrice)
                .HasMaxLength(50)
                .HasColumnName("CodeUFONegociatrice");
            entity.Property(e => e.DateDebutImpact).HasMaxLength(10);
            entity.Property(e => e.DateFinImpact).HasMaxLength(10);
            entity.Property(e => e.DeviseEncours).HasMaxLength(50);
            entity.Property(e => e.Elcredit)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("ELCredit");
            entity.Property(e => e.Eldilution)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("ELDilution");
            entity.Property(e => e.FixiteTauxContrat).HasMaxLength(2);
            entity.Property(e => e.FlagEncoursMcDo).HasMaxLength(50);
            entity.Property(e => e.IdentifiantContrat).HasMaxLength(30);
            entity.Property(e => e.MontantEncours).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.NumTirage).HasMaxLength(50);
            entity.Property(e => e.NumeroEncours).HasMaxLength(4);
            entity.Property(e => e.Pccencours)
                .HasMaxLength(9)
                .HasColumnName("PCCEncours");
            entity.Property(e => e.PeriodiciteInterets).HasMaxLength(2);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.TypeEncours).HasMaxLength(5);
            entity.Property(e => e.ValeurTauxFixe).HasColumnType("numeric(15, 0)");
        });

        modelBuilder.Entity<HeliosOngletTitre>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_ONGLET_TITRES");

            entity.Property(e => e.CodeDeviseNominal).HasMaxLength(2);
            entity.Property(e => e.CoursTitre).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.DateEcheanceTitre).HasMaxLength(10);
            entity.Property(e => e.DateNotation).HasMaxLength(10);
            entity.Property(e => e.DeviseCoupon).HasMaxLength(2);
            entity.Property(e => e.EligibiliteMcDo).HasMaxLength(1);
            entity.Property(e => e.FrequenceCotation).HasMaxLength(1);
            entity.Property(e => e.Granularite).HasMaxLength(1);
            entity.Property(e => e.IdentifiantTitre).HasMaxLength(16);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(16);
            entity.Property(e => e.IndexTaux).HasMaxLength(10);
            entity.Property(e => e.IndiceCotation).HasMaxLength(20);
            entity.Property(e => e.NatureTitre).HasMaxLength(2);
            entity.Property(e => e.NombreDecimaleCours).HasMaxLength(1);
            entity.Property(e => e.NominalTitre).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.NotationTitre).HasMaxLength(7);
            entity.Property(e => e.Oeec)
                .HasMaxLength(2)
                .HasColumnName("OEEC");
            entity.Property(e => e.OpcvmcomposeTitresEligibles)
                .HasMaxLength(1)
                .HasColumnName("OPCVMComposeTitresEligibles");
            entity.Property(e => e.PlaceCotation).HasMaxLength(3);
            entity.Property(e => e.Rafemetteur)
                .HasMaxLength(7)
                .HasColumnName("RAFEmetteur");
            entity.Property(e => e.RafgarantTitre)
                .HasMaxLength(7)
                .HasColumnName("RAFGarantTitre");
            entity.Property(e => e.RangTitre).HasMaxLength(1);
            entity.Property(e => e.TitreCote).HasMaxLength(1);
            entity.Property(e => e.TitreDette).HasMaxLength(1);
            entity.Property(e => e.TitreGaranti).HasMaxLength(1);
            entity.Property(e => e.TitreLiquide).HasMaxLength(1);
            entity.Property(e => e.TitreNote).HasMaxLength(1);
            entity.Property(e => e.TypeTaux).HasMaxLength(2);
            entity.Property(e => e.TypeTitre).HasMaxLength(1);
            entity.Property(e => e.ValeurOuMargeTauxFixe).HasColumnType("numeric(15, 0)");
        });

        modelBuilder.Entity<HeliosOngletTitresTmp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_ONGLET_TITRES_TMP");

            entity.Property(e => e.CodeDeviseNominal).HasMaxLength(2);
            entity.Property(e => e.CoursTitre).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.DateEcheanceTitre).HasMaxLength(10);
            entity.Property(e => e.DateNotation).HasMaxLength(10);
            entity.Property(e => e.DeviseCoupon).HasMaxLength(2);
            entity.Property(e => e.EligibiliteMcDo).HasMaxLength(1);
            entity.Property(e => e.FrequenceCotation).HasMaxLength(1);
            entity.Property(e => e.Granularite).HasMaxLength(1);
            entity.Property(e => e.IdentifiantTitre).HasMaxLength(16);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(16);
            entity.Property(e => e.IndexTaux).HasMaxLength(10);
            entity.Property(e => e.IndiceCotation).HasMaxLength(20);
            entity.Property(e => e.NatureTitre).HasMaxLength(2);
            entity.Property(e => e.NombreDecimaleCours).HasMaxLength(1);
            entity.Property(e => e.NominalTitre).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.NotationTitre).HasMaxLength(7);
            entity.Property(e => e.Oeec)
                .HasMaxLength(2)
                .HasColumnName("OEEC");
            entity.Property(e => e.OpcvmcomposeTitresEligibles)
                .HasMaxLength(1)
                .HasColumnName("OPCVMComposeTitresEligibles");
            entity.Property(e => e.PlaceCotation).HasMaxLength(3);
            entity.Property(e => e.Rafemetteur)
                .HasMaxLength(7)
                .HasColumnName("RAFEmetteur");
            entity.Property(e => e.RafgarantTitre)
                .HasMaxLength(7)
                .HasColumnName("RAFGarantTitre");
            entity.Property(e => e.RangTitre).HasMaxLength(1);
            entity.Property(e => e.TitreCote).HasMaxLength(1);
            entity.Property(e => e.TitreDette).HasMaxLength(1);
            entity.Property(e => e.TitreGaranti).HasMaxLength(1);
            entity.Property(e => e.TitreLiquide).HasMaxLength(1);
            entity.Property(e => e.TitreNote).HasMaxLength(1);
            entity.Property(e => e.TypeTaux).HasMaxLength(2);
            entity.Property(e => e.TypeTitre).HasMaxLength(1);
            entity.Property(e => e.ValeurOuMargeTauxFixe).HasColumnType("numeric(15, 0)");
        });

        modelBuilder.Entity<HeliosOngletTitresTocsv>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_ONGLET_TITRES_TOCSV");

            entity.Property(e => e.CodeDeviseNominal).HasMaxLength(2);
            entity.Property(e => e.CoursTitre).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.DateEcheanceTitre).HasMaxLength(10);
            entity.Property(e => e.DateNotation).HasMaxLength(10);
            entity.Property(e => e.DeviseCoupon).HasMaxLength(2);
            entity.Property(e => e.EligibiliteMcDo).HasMaxLength(1);
            entity.Property(e => e.FrequenceCotation).HasMaxLength(1);
            entity.Property(e => e.Granularite).HasMaxLength(1);
            entity.Property(e => e.IdentifiantTitre).HasMaxLength(16);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(16);
            entity.Property(e => e.IndexTaux).HasMaxLength(10);
            entity.Property(e => e.IndiceCotation).HasMaxLength(20);
            entity.Property(e => e.NatureTitre).HasMaxLength(2);
            entity.Property(e => e.NombreDecimaleCours).HasMaxLength(1);
            entity.Property(e => e.NominalTitre).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.NotationTitre).HasMaxLength(7);
            entity.Property(e => e.Oeec)
                .HasMaxLength(2)
                .HasColumnName("OEEC");
            entity.Property(e => e.OpcvmcomposeTitresEligibles)
                .HasMaxLength(1)
                .HasColumnName("OPCVMComposeTitresEligibles");
            entity.Property(e => e.PlaceCotation).HasMaxLength(3);
            entity.Property(e => e.Rafemetteur)
                .HasMaxLength(7)
                .HasColumnName("RAFEmetteur");
            entity.Property(e => e.RafgarantTitre)
                .HasMaxLength(7)
                .HasColumnName("RAFGarantTitre");
            entity.Property(e => e.RangTitre).HasMaxLength(1);
            entity.Property(e => e.TitreCote).HasMaxLength(1);
            entity.Property(e => e.TitreDette).HasMaxLength(1);
            entity.Property(e => e.TitreGaranti).HasMaxLength(1);
            entity.Property(e => e.TitreLiquide).HasMaxLength(1);
            entity.Property(e => e.TitreNote).HasMaxLength(1);
            entity.Property(e => e.TypeTaux).HasMaxLength(2);
            entity.Property(e => e.TypeTitre).HasMaxLength(1);
            entity.Property(e => e.ValeurOuMargeTauxFixe).HasColumnType("numeric(15, 0)");
        });

        modelBuilder.Entity<HeliosParamCategorieSyntheseAForcer>(entity =>
        {
            entity.HasKey(e => new { e.Source, e.CompteMagnitude });

            entity.ToTable("HELIOS_PARAM_CATEGORIE_SYNTHESE_A_FORCER");

            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(6);
            entity.Property(e => e.CategorieSynthese).HasMaxLength(20);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
        });

        modelBuilder.Entity<HeliosParamCategorieSyntheseAModifier>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_PARAM_CATEGORIE_SYNTHESE_A_MODIFIER");

            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.ValeurAajouter)
                .HasMaxLength(10)
                .HasColumnName("ValeurAAjouter");
        });

        modelBuilder.Entity<HeliosParamClo>(entity =>
        {
            entity.HasKey(e => e.ReferenceContratPivi);

            entity.ToTable("HELIOS_PARAM_CLO");

            entity.Property(e => e.ReferenceContratPivi)
                .HasMaxLength(30)
                .HasColumnName("ReferenceContratPIVI");
            entity.Property(e => e.AgenceCloture).HasMaxLength(2);
            entity.Property(e => e.AgenceOctroi).HasMaxLength(2);
            entity.Property(e => e.AssetId)
                .HasMaxLength(30)
                .HasColumnName("AssetID");
            entity.Property(e => e.DateCloture).HasMaxLength(10);
            entity.Property(e => e.DateMaturite).HasMaxLength(10);
            entity.Property(e => e.DateOctroi).HasMaxLength(10);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.Moodys).HasMaxLength(5);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RatingCloture).HasMaxLength(5);
            entity.Property(e => e.RatingOctroi).HasMaxLength(5);
            entity.Property(e => e.SandP).HasMaxLength(5);
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosParamCodesProduit>(entity =>
        {
            entity.HasKey(e => e.CodeProduit);

            entity.ToTable("HELIOS_PARAM_CODES_PRODUIT");

            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CodeSegmentMcDo).HasMaxLength(2);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.NatureTitre).HasMaxLength(2);
            entity.Property(e => e.OpcvmcomposeTitresEligibles)
                .HasMaxLength(1)
                .HasColumnName("OPCVMComposeTitresEligibles");
            entity.Property(e => e.SensOperation).HasMaxLength(1);
            entity.Property(e => e.TitreDette).HasMaxLength(1);
            entity.Property(e => e.TypeParticipation).HasMaxLength(1);
            entity.Property(e => e.TypeTitre).HasMaxLength(1);
            entity.Property(e => e.ValeurMobiliere).HasMaxLength(1);
        });

        modelBuilder.Entity<HeliosParamComptesMagnitudeAExtraire>(entity =>
        {
            entity.HasKey(e => e.CompteMagnitude);

            entity.ToTable("HELIOS_PARAM_COMPTES_MAGNITUDE_A_EXTRAIRE");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(6);
            entity.Property(e => e.CompteAnetter)
                .HasMaxLength(1)
                .HasColumnName("CompteANetter");
            entity.Property(e => e.CompteMagnitudeRemplacement).HasMaxLength(6);
            entity.Property(e => e.DetailsParPartnerId)
                .HasMaxLength(1)
                .HasColumnName("DetailsParPartnerID");
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.LibellePartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibellePartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(12)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
        });

        modelBuilder.Entity<HeliosParamEncoursElementaire>(entity =>
        {
            entity.HasKey(e => new { e.CodeProduit, e.Pcec });

            entity.ToTable("HELIOS_PARAM_ENCOURS_ELEMENTAIRES");

            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.Pcec)
                .HasMaxLength(5)
                .HasColumnName("PCEC");
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.NumeroEncours1).HasMaxLength(4);
            entity.Property(e => e.NumeroEncours2).HasMaxLength(4);
            entity.Property(e => e.NumeroEncours3).HasMaxLength(4);
            entity.Property(e => e.Pcecencours1)
                .HasMaxLength(5)
                .HasColumnName("PCECEncours1");
            entity.Property(e => e.Pcecencours2)
                .HasMaxLength(5)
                .HasColumnName("PCECEncours2");
            entity.Property(e => e.Pcecencours3)
                .HasMaxLength(5)
                .HasColumnName("PCECEncours3");
            entity.Property(e => e.TypeEncours1).HasMaxLength(5);
            entity.Property(e => e.TypeEncours2).HasMaxLength(5);
            entity.Property(e => e.TypeEncours3).HasMaxLength(5);
        });

        modelBuilder.Entity<HeliosParamEncoursElementairesPcc>(entity =>
        {
            entity.HasKey(e => new { e.CodeProduit, e.Pcc });

            entity.ToTable("HELIOS_PARAM_ENCOURS_ELEMENTAIRES_PCC");

            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.Pcc)
                .HasMaxLength(9)
                .HasColumnName("PCC");
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.NumeroEncours1).HasMaxLength(4);
            entity.Property(e => e.NumeroEncours2).HasMaxLength(4);
            entity.Property(e => e.NumeroEncours3).HasMaxLength(4);
            entity.Property(e => e.Pccencours1)
                .HasMaxLength(9)
                .HasColumnName("PCCEncours1");
            entity.Property(e => e.Pccencours2)
                .HasMaxLength(9)
                .HasColumnName("PCCEncours2");
            entity.Property(e => e.Pccencours3)
                .HasMaxLength(9)
                .HasColumnName("PCCEncours3");
            entity.Property(e => e.TypeEncours1).HasMaxLength(5);
            entity.Property(e => e.TypeEncours2).HasMaxLength(5);
            entity.Property(e => e.TypeEncours3).HasMaxLength(5);
        });

        modelBuilder.Entity<HeliosParamEntreesPerimetre>(entity =>
        {
            entity.HasKey(e => e.ReportingUnit);

            entity.ToTable("HELIOS_PARAM_ENTREES_PERIMETRE");

            entity.Property(e => e.ReportingUnit).HasMaxLength(5);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.Libelle).HasMaxLength(255);
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosParamIntentionGestionParCompte>(entity =>
        {
            entity.HasKey(e => e.CompteMagnitude);

            entity.ToTable("HELIOS_PARAM_INTENTION_GESTION_PAR_COMPTE");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.PortefeuilleIas)
                .HasMaxLength(255)
                .HasColumnName("PortefeuilleIAS");
        });

        modelBuilder.Entity<HeliosParamMappingMagnitudeCopernicUliss>(entity =>
        {
            entity.HasKey(e => new { e.CompteMagnitude, e.RefCategorieRwa });

            entity.ToTable("HELIOS_PARAM_MAPPING_MAGNITUDE_COPERNIC_ULISS");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(6);
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.AjoutDeLignes).HasMaxLength(1);
            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CodeProduit2).HasMaxLength(5);
            entity.Property(e => e.CodeProduit3).HasMaxLength(5);
            entity.Property(e => e.CompteCopernic).HasMaxLength(9);
            entity.Property(e => e.CompteCopernic2).HasMaxLength(9);
            entity.Property(e => e.CompteCopernic3).HasMaxLength(9);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.Pcec)
                .HasMaxLength(5)
                .HasColumnName("PCEC");
            entity.Property(e => e.Pcec2)
                .HasMaxLength(5)
                .HasColumnName("PCEC2");
            entity.Property(e => e.Pcec3)
                .HasMaxLength(5)
                .HasColumnName("PCEC3");
            entity.Property(e => e.RefCategorieRwa2)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA2");
            entity.Property(e => e.RefCategorieRwa3)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA3");
            entity.Property(e => e.Signe).HasMaxLength(2);
            entity.Property(e => e.Signe2).HasMaxLength(2);
            entity.Property(e => e.Signe3).HasMaxLength(2);
        });

        modelBuilder.Entity<HeliosParamMappingTitresMagnitude>(entity =>
        {
            entity.HasKey(e => e.PartnerId);

            entity.ToTable("HELIOS_PARAM_MAPPING_TITRES_MAGNITUDE");

            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.CategorieSynthese).HasMaxLength(20);
            entity.Property(e => e.FrequenceCotation).HasMaxLength(1);
            entity.Property(e => e.IdentifiantIsin)
                .HasMaxLength(12)
                .HasColumnName("IdentifiantISIN");
            entity.Property(e => e.IndiceCotation).HasMaxLength(20);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.LibellePartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibellePartnerID");
            entity.Property(e => e.LibelleTypeDette).HasMaxLength(12);
            entity.Property(e => e.ManagementIntent).HasMaxLength(2);
            entity.Property(e => e.PlaceCotation).HasMaxLength(3);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.TitreCote).HasMaxLength(1);
            entity.Property(e => e.TitreLiquide).HasMaxLength(1);
            entity.Property(e => e.TraitementStandard).HasMaxLength(1);
        });

        modelBuilder.Entity<HeliosParamMappingTitresTransparence>(entity =>
        {
            entity.HasKey(e => e.IdentifiantIsin);

            entity.ToTable("HELIOS_PARAM_MAPPING_TITRES_TRANSPARENCE");

            entity.Property(e => e.IdentifiantIsin)
                .HasMaxLength(12)
                .HasColumnName("IdentifiantISIN");
            entity.Property(e => e.FrequenceCotation).HasMaxLength(1);
            entity.Property(e => e.IndiceCotation).HasMaxLength(20);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.NomActif).HasMaxLength(255);
            entity.Property(e => e.PlaceCotation).HasMaxLength(3);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.TitreCote).HasMaxLength(1);
            entity.Property(e => e.TitreLiquide).HasMaxLength(1);
        });

        modelBuilder.Entity<HeliosParamPartnersAExclureParCompteMagnitude>(entity =>
        {
            entity.HasKey(e => new { e.CompteMagnitude, e.PartnerId });

            entity.ToTable("HELIOS_PARAM_PARTNERS_A_EXCLURE_PAR_COMPTE_MAGNITUDE");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(6);
            entity.Property(e => e.PartnerId)
                .HasMaxLength(12)
                .HasColumnName("PartnerID");
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
        });

        modelBuilder.Entity<HeliosParamPerimetreDette>(entity =>
        {
            entity.HasKey(e => e.CodeProduit);

            entity.ToTable("HELIOS_PARAM_PERIMETRE_DETTE");

            entity.HasIndex(e => e.CodeProduit, "IX_HELIOS_PARAM_PERIMETRE_DETTE").IsUnique();

            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.LibelleProduit).HasMaxLength(80);
        });

        modelBuilder.Entity<HeliosParamRatingTitrisation>(entity =>
        {
            entity.HasKey(e => e.IdentifiantIsin);

            entity.ToTable("HELIOS_PARAM_RATING_TITRISATIONS");

            entity.Property(e => e.IdentifiantIsin)
                .HasMaxLength(12)
                .HasColumnName("IdentifiantISIN");
            entity.Property(e => e.LastUpdate).HasMaxLength(40);
            entity.Property(e => e.Oeec)
                .HasMaxLength(2)
                .HasColumnName("OEEC");
            entity.Property(e => e.Rating).HasMaxLength(4);
        });

        modelBuilder.Entity<HeliosPrepReportingRwaAgregee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_PREP_REPORTING_RWA_AGREGEE");

            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CompteCopernic).HasMaxLength(9);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigineOuPartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibelleOrigineOuPartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Pcec)
                .HasMaxLength(5)
                .HasColumnName("PCEC");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosPrepReportingRwaNonAgregee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_PREP_REPORTING_RWA_NON_AGREGEE");

            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CompteCopernic).HasMaxLength(9);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigineOuPartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibelleOrigineOuPartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Pcec)
                .HasMaxLength(5)
                .HasColumnName("PCEC");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosPrepSyntheseRwaAgregee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_PREP_SYNTHESE_RWA_AGREGEE");

            entity.Property(e => e.Approche).HasMaxLength(255);
            entity.Property(e => e.CategorieProduit).HasMaxLength(255);
            entity.Property(e => e.CategorieSynthese).HasMaxLength(255);
            entity.Property(e => e.ClasseActifB2).HasMaxLength(255);
            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CompteCopernic).HasMaxLength(9);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigineOuPartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibelleOrigineOuPartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Pcec)
                .HasMaxLength(5)
                .HasColumnName("PCEC");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.Rafb2c)
                .HasMaxLength(7)
                .HasColumnName("RAFB2C");
            entity.Property(e => e.Rating).HasMaxLength(255);
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(6);
            entity.Property(e => e.Transparence).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosPrepSyntheseRwaNonAgregee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_PREP_SYNTHESE_RWA_NON_AGREGEE");

            entity.Property(e => e.Approche).HasMaxLength(255);
            entity.Property(e => e.CategorieProduit).HasMaxLength(255);
            entity.Property(e => e.CategorieSynthese).HasMaxLength(255);
            entity.Property(e => e.ClasseActifB2).HasMaxLength(255);
            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CompteCopernic).HasMaxLength(9);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigineOuPartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibelleOrigineOuPartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Pcec)
                .HasMaxLength(5)
                .HasColumnName("PCEC");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.Rafb2c)
                .HasMaxLength(7)
                .HasColumnName("RAFB2C");
            entity.Property(e => e.Rating).HasMaxLength(255);
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.Transparence).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosPrepSyntheseRwaOld>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_PREP_SYNTHESE_RWA_OLD");

            entity.Property(e => e.Approche).HasMaxLength(255);
            entity.Property(e => e.CategorieProduit).HasMaxLength(255);
            entity.Property(e => e.CategorieSynthese).HasMaxLength(255);
            entity.Property(e => e.ClasseActifB2).HasMaxLength(255);
            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CompteCopernic).HasMaxLength(9);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigineOuPartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibelleOrigineOuPartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Pcec)
                .HasMaxLength(5)
                .HasColumnName("PCEC");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.Rafb2c)
                .HasMaxLength(7)
                .HasColumnName("RAFB2C");
            entity.Property(e => e.Rating).HasMaxLength(255);
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(6);
            entity.Property(e => e.Transparence).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle0>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE0");

            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(12);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.TypeRaf)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("TypeRAF");
        });

        modelBuilder.Entity<HeliosReportControle0a>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE0A");

            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibellePartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibellePartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.TypeRaf)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("TypeRAF");
        });

        modelBuilder.Entity<HeliosReportControle0b>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE0B");

            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(12);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.TypeRaf)
                .HasMaxLength(37)
                .IsUnicode(false)
                .HasColumnName("TypeRAF");
        });

        modelBuilder.Entity<HeliosReportControle0c>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE0C");

            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE1");

            entity.Property(e => e.FondParametre)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.MontantDisponible)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle10>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE10");

            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CompteCopernic).HasMaxLength(9);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigineOuPartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibelleOrigineOuPartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Pcec)
                .HasMaxLength(5)
                .HasColumnName("PCEC");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle2>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE2");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle3>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE3");

            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle34>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE34");

            entity.Property(e => e.Approche).HasMaxLength(255);
            entity.Property(e => e.CategorieProduit).HasMaxLength(255);
            entity.Property(e => e.CategorieSynthese).HasMaxLength(255);
            entity.Property(e => e.ClasseActifB2).HasMaxLength(255);
            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CompteCopernic).HasMaxLength(9);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigineOuPartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibelleOrigineOuPartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Pcec)
                .HasMaxLength(5)
                .HasColumnName("PCEC");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.Rafb2c)
                .HasMaxLength(7)
                .HasColumnName("RAFB2C");
            entity.Property(e => e.Rating).HasMaxLength(255);
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(6);
            entity.Property(e => e.Transparence).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle35>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE35");

            entity.Property(e => e.Approche).HasMaxLength(255);
            entity.Property(e => e.CategorieProduit).HasMaxLength(255);
            entity.Property(e => e.ClasseActifB2).HasMaxLength(255);
            entity.Property(e => e.ExternalId)
                .HasMaxLength(255)
                .HasColumnName("ExternalID");
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Rafb2c)
                .HasMaxLength(7)
                .HasColumnName("RAFB2C");
            entity.Property(e => e.Rating).HasMaxLength(255);
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle36>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE36");

            entity.Property(e => e.IdentifiantB2c)
                .HasMaxLength(255)
                .HasColumnName("IdentifiantB2C");
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.MontantActifPondereB2c).HasColumnName("MontantActifPondereB2C");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle4>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE4");

            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle4a>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE4A");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle6>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE6");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibellePartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibellePartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle8>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE8");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibellePartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibellePartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosReportControle9>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_REPORT_CONTROLE9");

            entity.Property(e => e.Commentaires)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Source).HasMaxLength(10);
        });

        modelBuilder.Entity<HeliosSeedMoney>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_SEED_MONEY");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(6);
            entity.Property(e => e.Libelle).HasMaxLength(255);
            entity.Property(e => e.PartnerId)
                .HasMaxLength(6)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosTempExportSyntheseRwa>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_TEMP_EXPORT_SYNTHESE_RWA");

            entity.Property(e => e.ApprocheT1).HasMaxLength(255);
            entity.Property(e => e.ApprocheT2).HasMaxLength(255);
            entity.Property(e => e.CategorieProduitT1).HasMaxLength(255);
            entity.Property(e => e.CategorieProduitT2).HasMaxLength(255);
            entity.Property(e => e.CategorieSyntheseT1).HasMaxLength(255);
            entity.Property(e => e.CategorieSyntheseT2).HasMaxLength(255);
            entity.Property(e => e.ClasseActifB2t1)
                .HasMaxLength(255)
                .HasColumnName("ClasseActifB2T1");
            entity.Property(e => e.ClasseActifB2t2)
                .HasMaxLength(255)
                .HasColumnName("ClasseActifB2T2");
            entity.Property(e => e.CompteCopernic).HasMaxLength(9);
            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(13);
            entity.Property(e => e.LibelleOrigine).HasMaxLength(255);
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.Rafb2ct1)
                .HasMaxLength(7)
                .HasColumnName("RAFB2CT1");
            entity.Property(e => e.Rafb2ct2)
                .HasMaxLength(7)
                .HasColumnName("RAFB2CT2");
            entity.Property(e => e.RatingT1).HasMaxLength(255);
            entity.Property(e => e.RatingT2).HasMaxLength(255);
            entity.Property(e => e.Source).HasMaxLength(6);
            entity.Property(e => e.TraitementParTransparenceT1).HasMaxLength(3);
            entity.Property(e => e.TraitementParTransparenceT2).HasMaxLength(3);
            entity.Property(e => e.VariationBruteRwa).HasColumnName("VariationBruteRWA");
            entity.Property(e => e.VariationRelativeRwa).HasColumnName("VariationRelativeRWA");
        });

        modelBuilder.Entity<HeliosTempImportMagnitudeRetraite>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_TEMP_IMPORT_MAGNITUDE_RETRAITE");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(7);
            entity.Property(e => e.IdentifiantUniqueRetenu)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.LibellePartnerId)
                .HasMaxLength(255)
                .HasColumnName("LibellePartnerID");
            entity.Property(e => e.PartnerId)
                .HasMaxLength(15)
                .HasColumnName("PartnerID");
            entity.Property(e => e.PeriodeCloture).HasMaxLength(6);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.RefCategorieRwa)
                .HasMaxLength(3)
                .HasColumnName("RefCategorieRWA");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosTmpSyntheseRwaOld>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HELIOS_TMP_SYNTHESE_RWA_OLD");

            entity.Property(e => e.CompteMagnitude).HasMaxLength(12);
            entity.Property(e => e.PartnerId)
                .HasMaxLength(12)
                .HasColumnName("PartnerID");
            entity.Property(e => e.Source).HasMaxLength(3);
        });

        modelBuilder.Entity<HeliosTypeLog>(entity =>
        {
            entity.HasKey(e => e.IdTypeLog);

            entity.ToTable("HELIOS_TYPE_LOG");

            entity.Property(e => e.Type).HasMaxLength(20);
        });

        modelBuilder.Entity<RwaTrace>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RWA_TRACE");

            entity.Property(e => e.Dt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("DT");
            entity.Property(e => e.Info)
                .HasMaxLength(250)
                .HasColumnName("INFO");
            entity.Property(e => e.Login)
                .HasMaxLength(250)
                .HasColumnName("LOGIN");
            entity.Property(e => e.Section)
                .HasMaxLength(250)
                .HasColumnName("SECTION");
            entity.Property(e => e.Step)
                .HasMaxLength(250)
                .HasColumnName("STEP");
        });

        modelBuilder.Entity<VwHeliosOngletContratsCsv>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_HELIOS_ONGLET_CONTRATS_CSV");

            entity.Property(e => e.CodeActivite).HasMaxLength(50);
            entity.Property(e => e.CodeEtablissement).HasMaxLength(50);
            entity.Property(e => e.CodeProduit).HasMaxLength(5);
            entity.Property(e => e.CodeSegmentMcDo).HasMaxLength(2);
            entity.Property(e => e.CodeUfonegociatrice)
                .HasMaxLength(50)
                .HasColumnName("CodeUFONegociatrice");
            entity.Property(e => e.DateDebutFinancement).HasMaxLength(10);
            entity.Property(e => e.DateEffet).HasMaxLength(10);
            entity.Property(e => e.DateFinContrat).HasMaxLength(10);
            entity.Property(e => e.DateInitialeFinContrat).HasMaxLength(10);
            entity.Property(e => e.DateSignatureContrat).HasMaxLength(10);
            entity.Property(e => e.Devise).HasMaxLength(3);
            entity.Property(e => e.Diversifiee).HasMaxLength(1);
            entity.Property(e => e.Fcec)
                .HasMaxLength(4000)
                .HasColumnName("FCEC");
            entity.Property(e => e.FlagComptesSegregues).HasMaxLength(1);
            entity.Property(e => e.IdentifiantContrat).HasMaxLength(30);
            entity.Property(e => e.IdentifiantIsin)
                .HasMaxLength(16)
                .HasColumnName("IdentifiantISIN");
            entity.Property(e => e.IndicateurPerimetreMcDo).HasMaxLength(1);
            entity.Property(e => e.LibelleTypeDette).HasMaxLength(15);
            entity.Property(e => e.MethodeBaleIi)
                .HasMaxLength(7)
                .HasColumnName("MethodeBaleII");
            entity.Property(e => e.MotifExclusionMcDo).HasMaxLength(2);
            entity.Property(e => e.NbEncoursElementaires).HasMaxLength(4000);
            entity.Property(e => e.NombreGaranties).HasMaxLength(4000);
            entity.Property(e => e.PaysRisque).HasMaxLength(2);
            entity.Property(e => e.PortefeuilleIas)
                .HasMaxLength(3)
                .HasColumnName("PortefeuilleIAS");
            entity.Property(e => e.PresenceCreditPartage).HasMaxLength(1);
            entity.Property(e => e.PresenceGarantie).HasMaxLength(50);
            entity.Property(e => e.PrixExerciceOption).HasMaxLength(4000);
            entity.Property(e => e.PrixNegocie).HasMaxLength(4000);
            entity.Property(e => e.PrixUnitaireContrat).HasMaxLength(4000);
            entity.Property(e => e.ProduitAs400)
                .HasMaxLength(6)
                .HasColumnName("ProduitAS400");
            entity.Property(e => e.PtfReglementaireMcDo).HasMaxLength(7);
            entity.Property(e => e.Quantite).HasMaxLength(17);
            entity.Property(e => e.Raftiers)
                .HasMaxLength(7)
                .HasColumnName("RAFTiers");
            entity.Property(e => e.ReferenceContratNettingReglem).HasMaxLength(30);
            entity.Property(e => e.ReferenceMontage).HasMaxLength(8);
            entity.Property(e => e.Section).HasMaxLength(2);
            entity.Property(e => e.SensOperation).HasMaxLength(1);
            entity.Property(e => e.StrategieFrench).HasMaxLength(3);
            entity.Property(e => e.TauxLgd)
                .HasMaxLength(4000)
                .HasColumnName("TauxLGD");
            entity.Property(e => e.TopTransparence).HasMaxLength(50);
            entity.Property(e => e.TypeContratNetting).HasMaxLength(1);
            entity.Property(e => e.TypeOption).HasMaxLength(2);
            entity.Property(e => e.TypeParticipation).HasMaxLength(1);
        });

        modelBuilder.Entity<VwHeliosOngletEncoursCsv>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_HELIOS_ONGLET_ENCOURS_CSV");

            entity.Property(e => e.CodeEtablissement).HasMaxLength(50);
            entity.Property(e => e.CodeUfonegociatrice)
                .HasMaxLength(50)
                .HasColumnName("CodeUFONegociatrice");
            entity.Property(e => e.DateDebutImpact).HasMaxLength(10);
            entity.Property(e => e.DateFinImpact).HasMaxLength(10);
            entity.Property(e => e.DeviseEncours)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Elcredit)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ELCredit");
            entity.Property(e => e.Eldilution)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ELDilution");
            entity.Property(e => e.FixiteTauxContrat).HasMaxLength(2);
            entity.Property(e => e.FlagEncoursMcDo).HasMaxLength(50);
            entity.Property(e => e.IdentifiantContrat).HasMaxLength(30);
            entity.Property(e => e.MontantEncours).HasColumnType("numeric(15, 0)");
            entity.Property(e => e.NumTirage)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.NumeroEncours).HasMaxLength(4);
            entity.Property(e => e.Pcecencours)
                .HasMaxLength(8)
                .HasColumnName("PCECEncours");
            entity.Property(e => e.PeriodiciteInterets).HasMaxLength(2);
            entity.Property(e => e.Raf)
                .HasMaxLength(7)
                .HasColumnName("RAF");
            entity.Property(e => e.TypeEncours).HasMaxLength(5);
            entity.Property(e => e.ValeurTauxFixe)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwHeliosOngletTitresCsv>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_HELIOS_ONGLET_TITRES_CSV");

            entity.Property(e => e.CodeDeviseNominal).HasMaxLength(2);
            entity.Property(e => e.CoursTitre)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.DateEcheanceTitre)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.DateNotation)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.DeviseCoupon).HasMaxLength(2);
            entity.Property(e => e.EligibiliteMcDo).HasMaxLength(1);
            entity.Property(e => e.FrequenceCotation).HasMaxLength(1);
            entity.Property(e => e.Granularite).HasMaxLength(1);
            entity.Property(e => e.IdentifiantTitre).HasMaxLength(16);
            entity.Property(e => e.IdentifiantUniqueRetenu).HasMaxLength(16);
            entity.Property(e => e.IndexTaux).HasMaxLength(10);
            entity.Property(e => e.IndiceCotation).HasMaxLength(20);
            entity.Property(e => e.NatureTitre).HasMaxLength(2);
            entity.Property(e => e.NombreDecimaleCours).HasMaxLength(1);
            entity.Property(e => e.NominalTitre)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.NotationTitre).HasMaxLength(7);
            entity.Property(e => e.Oeec)
                .HasMaxLength(2)
                .HasColumnName("OEEC");
            entity.Property(e => e.OpcvmcomposeTitresEligibles)
                .HasMaxLength(1)
                .HasColumnName("OPCVMComposeTitresEligibles");
            entity.Property(e => e.PlaceCotation).HasMaxLength(3);
            entity.Property(e => e.Rafemetteur)
                .HasMaxLength(7)
                .HasColumnName("RAFEmetteur");
            entity.Property(e => e.RafgarantTitre)
                .HasMaxLength(7)
                .HasColumnName("RAFGarantTitre");
            entity.Property(e => e.RangTitre).HasMaxLength(1);
            entity.Property(e => e.TitreCote).HasMaxLength(1);
            entity.Property(e => e.TitreDette).HasMaxLength(1);
            entity.Property(e => e.TitreGaranti).HasMaxLength(1);
            entity.Property(e => e.TitreLiquide).HasMaxLength(1);
            entity.Property(e => e.TitreNote).HasMaxLength(1);
            entity.Property(e => e.TypeTaux).HasMaxLength(2);
            entity.Property(e => e.TypeTitre).HasMaxLength(1);
            entity.Property(e => e.ValeurOuMargeTauxFixe)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WorkflowStep>(entity =>
        {
            entity.ToTable("HECATE_WORKFLOW_STEPS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
