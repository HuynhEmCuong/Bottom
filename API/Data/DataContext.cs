using System;
using Bottom_API.DTO;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {
            Database.SetCommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
        }
        public DbSet<WMS_Code> WMS_Code { get; set; }
        public DbSet<WMSB_RackLocation_Main> WMSB_RackLocation_Main { get; set; }
        public DbSet<WMSB_Packing_List> WMSB_Packing_List { get; set; }
        public DbSet<WMSB_PackingList_Detail> WMSB_PackingList_Detail { get; set; }
        public DbSet<WMSB_Material_Purchase> WMSB_Material_Purchase { get; set; }
        public DbSet<WMSB_Material_Missing> WMSB_Material_Missing { get; set; }
        public DbSet<WMSB_QRCode_Main> WMSB_QRCode_Main { get; set; }
        public DbSet<WMSB_QRCode_Detail> WMSB_QRCode_Detail { get; set; }
        public DbSet<VM_WMSB_Material_Purchase> VM_WMSB_Material_Purchase { get; set; }
        public DbSet<WMSB_Transaction_Main> WMSB_Transaction_Main { get; set; }
        public DbSet<WMSB_Transaction_Detail> WMSB_Transaction_Detail { get; set; }
        public DbSet<WMSB_Material_Sheet_Size> WMSB_Material_Sheet_Size { get; set; }
        public DbSet<VM_WMSB_MES_MO> VM_WMSB_MES_MO { get; set; }
        public DbSet<VM_WMSB_PO> VM_WMSB_PO { get; set; }
        public DbSet<KanbanByPo_Dto> KanbanByPo_Dto { get; set; }

        public DbSet<NSP_REPORT_DETAIL_NO_SIZE> NSP_REPORT_DETAIL_NO_SIZE { get; set; }
        public DbSet<NSP_REPORT_DETAIL_NO_SIZE_2ND> NSP_REPORT_DETAIL_NO_SIZE_2ND { get; set; }
        public DbSet<KanbanByPoDetailExcel_Dto> KanbanByPoDetailExcel_Dto { get; set; }
        public DbSet<ReportMatRecExcel_Dto> ReportMatRecExcel_Dto { get; set; }
        public DbSet<RackArea_Dto> RackArea_Dto { get; set; }
        public DbSet<NSP_RACKS_AREA_RACK_LIST> NSP_RACKS_AREA_RACK_LIST { get; set; }
        public DbSet<NSP_Receive_Material_Report> NSP_Receive_Material_Report { get; set; }
        public DbSet<KanbanByCategories_Dto> KanbanByCategories_Dto { get; set; }
        public DbSet<KanbanByCategoryDetail_Dto> KanbanByCategoryDetail_Dto { get; set; }
        public DbSet<KanbanByCategoryDetailByToolCode_Dto> KanbanByCategoryDetailByToolCode_Dto { get; set; }
        public DbSet<KanbanByCategoryDetailByPo_Dto> KanbanByCategoryDetailByPo_Dtos { get; set; }
        public DbSet<Rack_Detail_T3T2_Dto> Rack_Detail_T3T2_Dto { get; set; }
        public DbSet<HistoryReportInputDB> HistoryReportInputDB { get; set; }
        public DbSet<HistoryReportOutputDB> HistoryReportOutputDB { get; set; }
        public virtual DbSet<WMSB_Setting_Supplier> WMSB_Setting_Supplier { get; set; }
        public virtual DbSet<WMSB_Transfer_Form> WMSB_Transfer_Form { get; set; }
        public virtual DbSet<WMSB_Cache> WMSB_Cache { get; set; }
        public DbSet<StockCompare> StockCompare {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WMS_Code>().HasKey(x => new { x.Code_Type, x.Code_ID });
            modelBuilder.Entity<WMSB_Material_Purchase>().HasKey(x => new { x.Type, x.Purchase_No, x.MO_No, x.MO_Seq, x.Order_Size, x.Material_ID });
            modelBuilder.Entity<WMSB_QRCode_Main>().HasKey(x => new { x.QRCode_ID, x.QRCode_Version });
            modelBuilder.Entity<VM_WMSB_Material_Purchase>().HasKey(x => new
            {
                x.Plan_No,
                x.Purchase_No,
                x.Mat_,
                x.MO_Seq
            });
            modelBuilder.Entity<WMSB_Material_Sheet_Size>().HasKey(x => new
            {
                x.Manno,
                x.Cur_Ent,
                x.Material_ID,
                x.Order_Size
            });

            modelBuilder.Entity<VM_WMSB_MES_MO>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<VM_WMSB_PO>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<KanbanByPo_Dto>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<NSP_REPORT_DETAIL_NO_SIZE>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<NSP_REPORT_DETAIL_NO_SIZE_2ND>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<NSP_Receive_Material_Report>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ReportMatRecExcel_Dto>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<KanbanByPoDetailExcel_Dto>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<RackArea_Dto>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<NSP_RACKS_AREA_RACK_LIST>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<KanbanByPoDetailExcel_Dto>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<KanbanByPoDetailExcel_Dto>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<KanbanByCategories_Dto>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<KanbanByCategoryDetail_Dto>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<KanbanByCategoryDetailByToolCode_Dto>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<KanbanByCategoryDetailByPo_Dto>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<Rack_Detail_T3T2_Dto>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<HistoryReportInputDB>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<HistoryReportOutputDB>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<StockCompare>(entity => 
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<WMSB_Setting_Supplier>(entity =>
            {
                entity.HasKey(e => new { e.Factory, e.Supplier_No, e.Subcon_ID });

            });
            modelBuilder.Entity<WMSB_Transfer_Form>(entity =>
            {
                entity.HasKey(e => new { e.Collect_Trans_No, e.Transac_No });

            });
            modelBuilder.Entity<WMSB_Cache>(entity =>
            {
                entity.HasKey(e => new { e.MO_No, e.MO_Seq, e.Material_ID });

            });
        }
    }
}