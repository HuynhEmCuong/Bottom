using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.Models;
using Bottom_API.Helpers;
using System;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Linq;
using Bottom_API.ViewModel;
using LinqKit;
using Bottom_API.Data;
using Dapper;

namespace Bottom_API._Services.Services
{
    public class InputService : IInputService
    {
        private readonly IPackingListRepository _repoPackingList;
        private readonly IPackingListDetailRepository _repoPacKingListDetail;
        private readonly IQRCodeMainRepository _repoQRCodeMain;
        private readonly IQRCodeDetailRepository _repoQRCodeDetail;
        private readonly ITransactionMainRepo _repoTransactionMain;
        private readonly ITransactionDetailRepo _repoTransactionDetail;
        private readonly IMaterialMissingRepository _repoMaterialMissing;
        private readonly IMaterialPurchaseRepository _repoMatPurchase;
        private readonly IMaterialMissingRepository _repoMatMissing;
        private readonly IMaterialViewRepository _repoMaterialView;
        private readonly IRackLocationRepo _repoRacklocationRepo;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly IDatabaseConnectionFactory _database;
        public InputService(
            IPackingListRepository repoPackingList,
            IQRCodeMainRepository repoQRCodeMain,
            IQRCodeDetailRepository repoQRCodeDetail,
            ITransactionMainRepo repoTransactionMain,
            ITransactionDetailRepo repoTransactionDetail,
            IMaterialMissingRepository repoMaterialMissing,
            IMaterialPurchaseRepository repoMatPurchase,
            IMaterialMissingRepository repoMatMissing,
            IMaterialViewRepository repoMaterialView,
            IRackLocationRepo repoRacklocationRepo,
            IPackingListDetailRepository repoPacKingListDetail,
            IDatabaseConnectionFactory database,
            IMapper mapper, 
            MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoQRCodeMain = repoQRCodeMain;
            _repoQRCodeDetail = repoQRCodeDetail;
            _repoPackingList = repoPackingList;
            _repoTransactionMain = repoTransactionMain;
            _repoTransactionDetail = repoTransactionDetail;
            _repoMaterialMissing = repoMaterialMissing;
            _repoMatMissing = repoMatMissing;
            _repoMatPurchase = repoMatPurchase;
            _repoMaterialView = repoMaterialView;
            _repoRacklocationRepo = repoRacklocationRepo;
            _repoPacKingListDetail = repoPacKingListDetail;
            _database = database;
        }
        public async Task<Transaction_Dto> GetByQRCodeID(object qrCodeID)
        {
            Transaction_Dto model = new Transaction_Dto();
            var qrCodeModel = await _repoQRCodeMain.GetByQRCodeID(qrCodeID);
            if (qrCodeModel != null)
            {
                var packingListModel = await _repoPackingList.GetByReceiveNo(qrCodeModel.Receive_No);
                var listQrCodeDetails = await _repoQRCodeDetail.GetByQRCodeIDAndVersion(qrCodeID, qrCodeModel.QRCode_Version);
                decimal? num = 0;
                foreach (var item in listQrCodeDetails)
                {
                    num += item.Qty;
                }

                model.QrCode_Id = qrCodeModel.QRCode_ID.Trim();
                model.Plan_No = packingListModel.MO_No.Trim();
                model.Suplier_No = packingListModel.Supplier_ID.Trim();
                model.Suplier_Name = packingListModel.Supplier_Name.Trim();
                model.Batch = packingListModel.MO_Seq;
                model.Mat_Id = packingListModel.Material_ID.Trim();
                model.Mat_Name = packingListModel.Material_Name.Trim();
                model.Accumated_Qty = num;
                model.Trans_In_Qty = 0;
                model.InStock_Qty = 0;
            }

            return model;
        }

        public async Task<Transaction_Detail_Dto> GetDetailByQRCodeID(object qrCodeID)
        {
            Transaction_Detail_Dto model = new Transaction_Detail_Dto();
            var qrCodeMainList = await _repoQRCodeMain.CheckQrCodeID(qrCodeID);
            if (qrCodeMainList.Count == 0)
            {
                return null;
            }
            else
            {
                var qrCodeModel = qrCodeMainList.Where(x => x.Is_Scanned.Trim() == "N").FirstOrDefault();
                if (qrCodeModel != null)
                {
                    var packingListModel = await _repoPackingList.GetByReceiveNo(qrCodeModel.Receive_No);
                    var listQrCodeDetails = await _repoQRCodeDetail.GetByQRCodeIDAndVersion(qrCodeID, qrCodeModel.QRCode_Version);
                    decimal? num = 0;
                    List<DetailSize> listDetail = new List<DetailSize>();
                    foreach (var item in listQrCodeDetails)
                    {
                        DetailSize detail = new DetailSize();
                        num += item.Qty;
                        detail.Tool_Size = item.Tool_Size;
                        detail.Size = item.Order_Size;
                        detail.Qty = item.Qty;
                        listDetail.Add(detail);
                    }
                    model.Suplier_No = packingListModel.Supplier_ID;
                    model.Suplier_Name = packingListModel.Supplier_Name;
                    model.Detail_Size = listDetail;
                    model.QrCode_Id = qrCodeModel.QRCode_ID.Trim();
                    model.Plan_No = packingListModel.MO_No.Trim();
                    model.Batch = packingListModel.MO_Seq;
                    model.Mat_Id = packingListModel.Material_ID.Trim();
                    model.Mat_Name = packingListModel.Material_Name.Trim();
                    model.Accumated_Qty = num;
                    model.Trans_In_Qty = 0;
                    model.InStock_Qty = 0;
                    model.Is_Scanned = qrCodeModel.Is_Scanned;
                    return model;
                }
                else
                {
                    model.Is_Scanned = "Y";
                    return model;
                }
            }
        }

        public async Task<bool> CreateInput(Transaction_Detail_Dto model, string updateBy)
        {
            var qrCodeModel = await _repoQRCodeMain.GetByQRCodeID(model.QrCode_Id);
            if (qrCodeModel != null && qrCodeModel.Valid_Status == "Y")
            {
                var listQrCodeDetails = await _repoQRCodeDetail.GetByQRCodeIDAndVersion(qrCodeModel.QRCode_ID, qrCodeModel.QRCode_Version);
                var packingListModel = await _repoPackingList.GetByReceiveNo(qrCodeModel.Receive_No);
                WMSB_Transaction_Main inputModel = new WMSB_Transaction_Main();
                inputModel.Transac_Type = "I";
                inputModel.Transac_No = model.Input_No;
                inputModel.Transac_Time = DateTime.Now;
                inputModel.QRCode_ID = qrCodeModel.QRCode_ID;
                inputModel.QRCode_Version = qrCodeModel.QRCode_Version;
                inputModel.MO_No = packingListModel.MO_No;
                inputModel.MO_Seq = packingListModel.MO_Seq;
                inputModel.Purchase_No = packingListModel.Purchase_No;
                inputModel.Material_ID = packingListModel.Material_ID;
                inputModel.Material_Name = packingListModel.Material_Name;
                inputModel.Purchase_Qty = model.Accumated_Qty;
                inputModel.Transacted_Qty = model.Trans_In_Qty;
                inputModel.Rack_Location = model.Rack_Location;
                inputModel.Can_Move = "Y";
                inputModel.Is_Transfer_Form = "N";
                inputModel.Updated_By = updateBy;
                inputModel.Updated_Time = DateTime.Now;
                _repoTransactionMain.Add(inputModel);

                var i = 0;
                foreach (var item in model.Detail_Size)
                {
                    WMSB_Transaction_Detail inputDetailModel = new WMSB_Transaction_Detail();
                    inputDetailModel.Transac_No = inputModel.Transac_No;
                    inputDetailModel.Tool_Size = listQrCodeDetails[i].Tool_Size;
                    inputDetailModel.Model_Size = listQrCodeDetails[i].Model_Size;
                    inputDetailModel.Order_Size = listQrCodeDetails[i].Order_Size;
                    inputDetailModel.Spec_Size = listQrCodeDetails[i].Spec_Size;
                    inputDetailModel.Qty = listQrCodeDetails[i].Qty;
                    inputDetailModel.Trans_Qty = item.Qty;
                    inputDetailModel.Instock_Qty = item.Qty;
                    inputDetailModel.Untransac_Qty = inputDetailModel.Qty - inputDetailModel.Trans_Qty;
                    inputDetailModel.Updated_By = updateBy;
                    inputDetailModel.Updated_Time = DateTime.Now;
                    _repoTransactionDetail.Add(inputDetailModel);
                    i += 1;
                }
                return await _repoTransactionMain.SaveAll();
            }
            return false;
        }

        public async Task<bool> SubmitInput(InputSubmitModel data, string updateBy)
        {
            foreach (var item in data.TransactionList)
            {
                await CreateInput(item, updateBy);
            }

            var lists = data.InputNoList;
            if (lists.Count > 0)
            {
                string Transac_Sheet_No;
                do
                {
                    string num = CodeUtility.RandomNumber(3);
                    Transac_Sheet_No = "IB" + DateTime.Now.ToString("yyyyMMdd") + num;
                } while (await _repoTransactionMain.CheckTranSheetNo(Transac_Sheet_No));

                foreach (var item in lists)
                {
                    string Missing_No;
                    do
                    {
                        // Để Mising No khác nhau 
                        string num2 = CodeUtility.RandomNumber(3);
                        Missing_No = "BTM" + DateTime.Now.ToString("yyyyMMdd") + num2;
                    } while (await this.CheckMissingMo(Missing_No));

                    WMSB_Transaction_Main model = await _repoTransactionMain.GetByInputNo(item);
                    model.Can_Move = "Y";
                    model.Transac_Sheet_No = Transac_Sheet_No;
                    model.Is_Transfer_Form = "N";
                    model.Updated_By = updateBy;
                    model.Updated_Time = DateTime.Now;
                    if (model.Purchase_Qty > model.Transacted_Qty)
                    {
                        model.Missing_No = Missing_No;

                        //Tạo mới record và update status record cũ trong bảng QRCode_Main và QRCode_Detail
                        GenerateNewQrCode(model.QRCode_ID, model.QRCode_Version, item, updateBy);

                        //Update QrCode Version cho bảng Transaction_Main
                        model.QRCode_Version += 1;

                        // Tạo mới record trong bảng Missing
                        CreateMissing(model.Purchase_No, model.MO_No, model.MO_Seq, model.Material_ID, model.Transac_No, model.Missing_No, updateBy);
                    }
                    else
                    {
                        WMSB_QRCode_Main qrModel = await _repoQRCodeMain.GetByQRCodeID(model.QRCode_ID);
                        qrModel.Is_Scanned = "Y";
                        _repoQRCodeMain.Update(qrModel);
                    }

                    _repoTransactionMain.Update(model);
                }
                return await _repoTransactionMain.SaveAll();
            }
            return false;
        }

        public async Task<MissingPrint_Dto> GetMaterialPrint(string missingNo)
        {
            var materialMissingModel = await _repoMaterialMissing.FindAll(x => x.Missing_No.Trim() == missingNo.Trim()).ProjectTo<Material_Dto>(_configMapper).FirstOrDefaultAsync();
            var transactionMainModel = _repoTransactionMain.FindSingle(x => x.Missing_No.Trim() == missingNo.Trim() && x.Transac_Type.Trim() == "I");
            var transactionDetailByMissingNo = await _repoTransactionDetail.FindAll(x => x.Transac_No.Trim() == transactionMainModel.Transac_No.Trim()).ProjectTo<TransferLocationDetail_Dto>(_configMapper).OrderBy(x => x.Tool_Size).ToListAsync();
            var materialPurchaseModel = await _repoMaterialView
                            .FindAll(   x => x.Plan_No.Trim() == materialMissingModel.MO_No.Trim() && 
                                        x.Purchase_No.Trim() == materialMissingModel.Purchase_No.Trim() && 
                                        x.Mat_.Trim() == materialMissingModel.Material_ID.Trim())
                            .FirstOrDefaultAsync();

            // nếu materialPurchaseModel rỗng thì  Custmoer_Name gán bằng rỗng
            materialMissingModel.Custmoer_Name = materialPurchaseModel == null ? "" : materialPurchaseModel.Custmoer_Name;
            // Lấy ra những thuộc tính cần in
            MissingPrint_Dto result = new MissingPrint_Dto();
            result.MaterialMissing = materialMissingModel;
            result.TransactionDetailByMissingNo = transactionDetailByMissingNo;

            return result;
        }
        private void CreateMissing(string Purchase_No, string MO_No, string MO_Seq, string Material_ID, string transacNo, string Missing_No, string updateBy)
        {
            //Lấy list Material Purchase
            var matPurchase = _repoMatPurchase.GetFactory(Purchase_No, MO_No, MO_Seq, Material_ID);

            //Lấy PackingList
            var packingList = _repoPackingList.GetPackingList(Purchase_No, MO_No, MO_Seq, Material_ID);
            //Lấy danh sách transaction detail
            List<WMSB_Transaction_Detail> listDetails = _repoTransactionDetail.GetListTransDetailByTransacNo(transacNo);
            foreach (var detail in listDetails)
            {
                WMSB_Material_Missing model = new WMSB_Material_Missing();
                model.Missing_No = Missing_No;
                model.Purchase_No = Purchase_No;
                model.MO_No = MO_No;
                model.MO_Seq = MO_Seq;
                model.Material_ID = packingList.Material_ID;
                model.Material_Name = packingList.Material_Name;
                model.Model_No = packingList.Model_No;
                model.Model_Name = packingList.Model_Name;
                model.Article = packingList.Article;
                model.Supplier_ID = packingList.Supplier_ID;
                model.Supplier_Name = packingList.Supplier_Name;
                model.Process_Code = packingList.Subcon_ID;
                model.Subcon_Name = packingList.Subcon_Name;
                model.T3_Supplier = packingList.T3_Supplier;
                model.T3_Supplier_Name = packingList.T3_Supplier_Name;
                model.Order_Size = detail.Order_Size;
                model.Model_Size = detail.Model_Size;
                model.Tool_Size = detail.Tool_Size;
                model.Spec_Size = detail.Spec_Size;
                model.Purchase_Qty = detail.Untransac_Qty;
                model.Accumlated_In_Qty = 0;
                model.Status = "N";
                model.Updated_Time = DateTime.Now;
                model.Updated_By = updateBy;
                foreach (var purchase in matPurchase)
                {
                    if (detail.Order_Size == purchase.Order_Size)
                    {
                        model.Factory_ID = purchase.Factory_ID;
                        model.MO_Qty = purchase.MO_Qty;
                        model.PreBook_Qty = purchase.PreBook_Qty;
                        model.Stock_Qty = purchase.Stock_Qty;
                        model.Require_Delivery = purchase.Require_Delivery;
                        model.Confirm_Delivery = purchase.Confirm_Delivery;
                        model.Custmoer_Part = purchase.Custmoer_Part;
                        model.T3_Purchase_No = purchase.T3_Purchase_No;
                        model.Stage = purchase.Stage;
                        model.Tool_ID = purchase.Tool_ID;
                        model.Tool_Type = purchase.Tool_Type;
                        model.Purchase_Kind = purchase.Purchase_Kind;
                        model.Collect_No = purchase.Collect_No;
                        model.Purchase_Size = purchase.Purchase_Size;
                    }
                }
                _repoMatMissing.Add(model);
            }
        }
        private void GenerateNewQrCode(string qrCodeID, int qrCodeVersion, string transacNo, string updateBy)
        {
            //Update dòng QRCodeMain cũ
            var qrCodeMain = _repoQRCodeMain.GetByQRCodeIDAndVersion(qrCodeID, qrCodeVersion);
            qrCodeMain.Valid_Status = "N";
            qrCodeMain.Is_Scanned = "Y";
            qrCodeMain.Invalid_Date = DateTime.Now;
            _repoQRCodeMain.Update(qrCodeMain);

            //Thêm QRCode mới và update version lên 
            WMSB_QRCode_Main model = new WMSB_QRCode_Main();
            model.QRCode_ID = qrCodeID;
            model.QRCode_Version = qrCodeMain.QRCode_Version + 1;
            model.QRCode_Type = qrCodeMain.QRCode_Type;
            model.Receive_No = qrCodeMain.Receive_No;
            model.Valid_Status = "Y";
            model.Is_Scanned = "Y";
            model.Updated_By = updateBy;
            model.Updated_Time = DateTime.Now;
            _repoQRCodeMain.Add(model);

            //Lấy danh sách transaction detail
            List<WMSB_Transaction_Detail> listDetails = _repoTransactionDetail.GetListTransDetailByTransacNo(transacNo);

            //Tạo mới các dòng QRCode Detail dựa trên QRcode version mới với Qty tương ứng với Trans_Qty của Transaction_Detail
            foreach (var item in listDetails)
            {
                WMSB_QRCode_Detail detailQRCode = new WMSB_QRCode_Detail();
                detailQRCode.QRCode_ID = qrCodeID;
                detailQRCode.QRCode_Version = model.QRCode_Version;
                detailQRCode.Tool_Size = item.Tool_Size;
                detailQRCode.Model_Size = item.Model_Size;
                detailQRCode.Order_Size = item.Order_Size;
                detailQRCode.Spec_Size = item.Spec_Size;
                detailQRCode.Qty = item.Trans_Qty;
                detailQRCode.Updated_By = updateBy;
                detailQRCode.Updated_Time = DateTime.Now;
                _repoQRCodeDetail.Add(detailQRCode);
            }
        }

        public async Task<PagedList<QrCodeAgain_Dto>> FilterQrCodeAgain(PaginationParams param, FilterQrCodeAgainParam filterParam)
        {
            var pred_List_Transaction_Main = PredicateBuilder.New<WMSB_Transaction_Main>(true);
            pred_List_Transaction_Main.And(x => x.Can_Move == "Y");

            if (!String.IsNullOrEmpty(filterParam.To_Date) && !String.IsNullOrEmpty(filterParam.From_Date))
            {
                pred_List_Transaction_Main.And(x => x.Transac_Time >= Convert.ToDateTime(filterParam.From_Date + " 00:00:00.000") &&
                    x.Transac_Time <= Convert.ToDateTime(filterParam.To_Date + " 23:59:59.997"));
            }
            if (!String.IsNullOrEmpty(filterParam.MO_No))
            {
                pred_List_Transaction_Main.And(x => x.MO_No.Trim() == filterParam.MO_No.Trim());
            }
            if (!String.IsNullOrEmpty(filterParam.Rack_Location))
            {
                pred_List_Transaction_Main.And(x => x.Rack_Location.Trim() == filterParam.Rack_Location.Trim());
            }
            if (!String.IsNullOrEmpty(filterParam.Material_ID))
            {
                pred_List_Transaction_Main.And(x => x.Material_ID.Trim() == filterParam.Material_ID.Trim());
            }
            var listTransactionMain =  _repoTransactionMain.FindAll(pred_List_Transaction_Main);
            var packingLists = _repoPackingList.FindAll();
            var data = (from a in listTransactionMain
                        join b in packingLists
on new { MO_No = a.MO_No.Trim(), Purchase_No = a.Purchase_No.Trim(), Mo_Seq = a.MO_Seq, Material_View_Dto = a.Material_ID.Trim() }
equals new { MO_No = b.MO_No.Trim(), Purchase_No = b.Purchase_No.Trim(), Mo_Seq = b.MO_Seq, Material_View_Dto = b.Material_ID.Trim() }
                        select new QrCodeAgain_Dto()
                        {
                            ID = a.ID,
                            Transac_Type = a.Transac_Type,
                            Transac_No = a.Transac_No,
                            Transac_Sheet_No = a.Transac_Sheet_No,
                            Can_Move = a.Can_Move,
                            Transac_Time = a.Transac_Time,
                            QRCode_ID = a.QRCode_ID,
                            QRCode_Version = a.QRCode_Version,
                            MO_No = a.MO_No,
                            Purchase_No = a.Purchase_No,
                            MO_Seq = a.MO_Seq,
                            Material_ID = a.Material_ID,
                            Material_Name = a.Material_Name,
                            Purchase_Qty = a.Purchase_Qty,
                            Transacted_Qty = a.Transacted_Qty,
                            Rack_Location = a.Rack_Location,
                            Missing_No = a.Missing_No,
                            Pickup_No = a.Pickup_No,
                            Supplier_ID = b.Supplier_ID,
                            Supplier_Name = b.Supplier_Name,
                            Updated_By = a.Updated_By,
                            Updated_Time = a.Updated_Time
                        });
            if (!String.IsNullOrEmpty(filterParam.Supplier_ID) && filterParam.Supplier_ID.Trim() != "All")
            {
                data = data.Where(x => x.Supplier_ID.Trim() == filterParam.Supplier_ID.Trim());
            }
            data = data.Distinct().OrderByDescending(x => x.Updated_Time);
            return await PagedList<QrCodeAgain_Dto>.CreateAsync(data, param.PageNumber, param.PageSize, false);
        }

        public async Task<string> FindMaterialName(string materialID)
        {
            if (materialID != null && materialID != string.Empty)
            {
                var materialModel = await _repoMaterialView.FindAll(x => x.Mat_.Trim() == materialID.Trim()).FirstOrDefaultAsync();
                if (materialModel != null)
                {
                    return materialModel.Mat__Name;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public async Task<PagedList<Transaction_Main_Dto>> FilterMissingPrint(PaginationParams param, FilterMissingParam filterParam)
        {
            var lists = _repoTransactionMain.GetAll()
                .ProjectTo<Transaction_Main_Dto>(_configMapper)
                .Where(x => x.Missing_No != null && x.Transac_Type.Trim() == "I");
            if (filterParam.MO_No != null && filterParam.MO_No != "")
            {
                lists = lists.Where(x => x.MO_No.Trim() == filterParam.MO_No.Trim());
            }
            if (filterParam.Material_ID != null && filterParam.Material_ID != "")
            {
                lists = lists.Where(x => x.Material_ID.Trim() == filterParam.Material_ID.Trim());
            }
            lists = lists.Distinct().OrderByDescending(x => x.Updated_Time);
            return await PagedList<Transaction_Main_Dto>.CreateAsync(lists, param.PageNumber, param.PageSize);
        }

        public async Task<string> FindMissingByQrCode(string qrCodeID)
        {
            var trans = await _repoTransactionMain
                            .FindAll(   x => x.QRCode_ID.Trim() == qrCodeID.Trim() &&
                                        (!String.IsNullOrEmpty(x.Missing_No)))
                    .OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
            return trans.Missing_No;
        }


        public async Task<bool> CheckQrCodeInV696(string qrCodeID)
        {
            var qrCodeMain = await _repoQRCodeMain.FindAll(x => x.Is_Scanned == "N" && x.QRCode_ID.Trim() == qrCodeID.Trim())
                .FirstOrDefaultAsync();
            if (qrCodeMain == null)
            {
                return false;
            }
            else
            {
                var packingModel = await _repoPackingList.FindAll(x => x.Receive_No.Trim() == qrCodeMain.Receive_No.Trim())
                    .FirstOrDefaultAsync();
                if (packingModel == null)
                {
                    return false;
                }
                else
                {
                    if (packingModel.Supplier_ID.Trim() == "V696")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        public async Task<bool> CheckRackLocation(string rackLocation)
        {
            var rackMain = await _repoRacklocationRepo.FindAll(x => x.Rack_Location.Trim() == rackLocation.Trim())
                .FirstOrDefaultAsync();
            if (rackMain == null)
            {
                return false;
            }
            else
            {
                if (rackMain.Area_ID.Trim() == "A012")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<bool> CheckMissingMo(string missingNo)
        {
            var transactionModel = await _repoTransactionMain.FindAll(x => x.Missing_No.Trim() == missingNo.Trim()).FirstOrDefaultAsync();
            if (transactionModel != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // ---Phần in QrCodeId sau khi Input (Sorting Form) ----------------------------//
        public async Task<WMSB_Transaction_Main> FindQrCodeInput(string qrCodeId)
        {
            var trans = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == qrCodeId.Trim())
                .OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
            return trans;
        }
        public async Task<PagedList<IntegrationInputModel>> SearchIntegrationInput(PaginationParams param, FilterPackingListParam filterparam)
        {
            var pred_Packing_Lists = PredicateBuilder.New<WMSB_Packing_List>(true);
            pred_Packing_Lists.And(x => x.Generated_QRCode.Trim() == "Y");
            if (!String.IsNullOrEmpty(filterparam.MO_No)) {
                pred_Packing_Lists.And(x => x.MO_No.Trim() == filterparam.MO_No.Trim());
            }
            if (filterparam.Supplier_ID != null && filterparam.Supplier_ID != "All") {
                pred_Packing_Lists.And(x => x.Supplier_ID.Trim() == filterparam.Supplier_ID.Trim());
            }
            if (filterparam.From_Date != null && filterparam.To_Date != null) {
                pred_Packing_Lists.And(x => x.Receive_Date >= Convert.ToDateTime(filterparam.From_Date + " 00:00:00.000") &&
                    x.Receive_Date <= Convert.ToDateTime(filterparam.To_Date + " 23:59:59.997"));
            }
            var packingLists = await _repoPackingList.FindAll(pred_Packing_Lists).ToListAsync();
            var conn = await _database.CreateConnectionAsync();
            var qrCodeMains = conn.Query<WMSB_QRCode_Main>("Select * from WMSB_QRCode_Main where Is_Scanned = 'N'").ToList();
            var qrCodeMainDetail = conn.Query<WMSB_QRCode_Detail>("Select * from WMSB_QRCode_Detail").ToList();
            var data = (from a in packingLists
                        join b in qrCodeMains
                        on a.Receive_No.Trim() equals b.Receive_No.Trim()
                        join c in qrCodeMainDetail on b.QRCode_ID.Trim() equals c.QRCode_ID.Trim()
                        select new IntegrationInputModel()
                        {
                            Receive_No = a.Receive_No,
                            QRCode_Version = b.QRCode_Version,
                            QRCode_ID = b.QRCode_ID,
                            MO_No = a.MO_No,
                            Purchase_No = a.Purchase_No,
                            MO_Seq = a.MO_Seq,
                            Material_ID = a.Material_ID,
                            Material_Name = a.Material_Name,
                            Supplier_ID = a.Supplier_ID,
                            Supplier_Name = a.Supplier_Name,
                            Receive_Qty = c.Qty
                        }).GroupBy(x => x.QRCode_ID).Select(z => new IntegrationInputModel()
                        {
                            Receive_No = z.FirstOrDefault().Receive_No,
                            QRCode_Version = z.FirstOrDefault().QRCode_Version,
                            Purchase_No = z.FirstOrDefault().Purchase_No,
                            QRCode_ID = z.FirstOrDefault().QRCode_ID,
                            MO_No = z.FirstOrDefault().MO_No,
                            MO_Seq = z.FirstOrDefault().MO_Seq,
                            Material_ID = z.FirstOrDefault().Material_ID,
                            Material_Name = z.FirstOrDefault().Material_Name,
                            Supplier_ID = z.FirstOrDefault().Supplier_ID,
                            Supplier_Name = z.FirstOrDefault().Supplier_Name,
                            Receive_Qty = z.Sum(k => k.Receive_Qty)
                        }).ToList();

            var receiveNos = data.Select(x => x.Receive_No.Trim()).ToList();
            var packingListDetails = conn
                                .Query<WMSB_PackingList_Detail>("Select * from WMSB_PackingList_Detail where Receive_No IN @receiveNos", new {receiveNos = receiveNos})
                                .ToList();
            foreach (var item in data)
            {
                item.PackingListDetailItem = packingListDetails.Where(x => x.Receive_No.Trim() == item.Receive_No.Trim()).ToList();
            }

            return PagedList<IntegrationInputModel>.Create(data, param.PageNumber, param.PageSize,false);
        }

        public async Task<bool> IntegrationInputSubmit(List<IntegrationInputModel> data, string user)
        {
            data = data.Where(x => !string.IsNullOrEmpty(x.Rack_Location)).ToList();
            foreach (var item in data)
            {
                // Update Table WMSB_QRCode_Main
                var qrCodeModels = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == item.QRCode_ID.Trim() &&
                                                    x.Is_Scanned.Trim() == "N" &&
                                                    x.Receive_No.Trim() == item.Receive_No.Trim()).FirstOrDefaultAsync();
                if (qrCodeModels != null)
                {
                    qrCodeModels.Is_Scanned = "Y";
                }


                //---------------------------- Check TransactionMain-------------------------------//
                var transationMainCheck = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == item.QRCode_ID.Trim() &&
                                                x.MO_No.Trim() == item.MO_No.Trim() &&
                                                x.Purchase_No.Trim() == item.Purchase_No.Trim() &&
                                                x.Material_ID.Trim() == item.Material_ID.Trim()).FirstOrDefaultAsync();
                if (transationMainCheck == null)
                {
                    string Transac_Sheet_No, Transac_No;
                    do
                    {
                        string num = CodeUtility.RandomNumber(3);
                        Transac_Sheet_No = "IB" + DateTime.Now.ToString("yyyyMMdd") + num;
                    } while (await _repoTransactionMain.CheckTranSheetNo(Transac_Sheet_No));
                    do
                    {
                        string num1 = CodeUtility.RandomNumber(3);
                        Transac_No = "IB" + item.MO_No.Trim() + num1;
                    } while (await _repoTransactionMain.CheckTransacNo(Transac_No));

                    // Tạo New Model để Add vào Table WMSB_Transaction_Main
                    var transactionMainModel = new WMSB_Transaction_Main();
                    transactionMainModel.Transac_Type = "I";
                    transactionMainModel.Transac_No = Transac_No;
                    transactionMainModel.Transac_Sheet_No = Transac_Sheet_No;
                    transactionMainModel.Can_Move = "Y";
                    transactionMainModel.Transac_Time = DateTime.Now;
                    transactionMainModel.QRCode_ID = item.QRCode_ID;
                    transactionMainModel.QRCode_Version = item.QRCode_Version;
                    transactionMainModel.MO_No = item.MO_No.Trim();
                    transactionMainModel.Purchase_No = item.Purchase_No.Trim();
                    transactionMainModel.MO_Seq = item.MO_Seq;
                    transactionMainModel.Material_ID = item.Material_ID.Trim();
                    transactionMainModel.Material_Name = item.Material_Name.Trim();
                    transactionMainModel.Purchase_Qty = item.Receive_Qty;
                    transactionMainModel.Transacted_Qty = item.Receive_Qty;
                    transactionMainModel.Rack_Location = item.Rack_Location;
                    transactionMainModel.Is_Transfer_Form = "N";
                    transactionMainModel.Updated_Time = DateTime.Now;
                    transactionMainModel.Updated_By = user;
                    _repoTransactionMain.Add(transactionMainModel);

                    // Add vào table WMSB_Transaction_Detail             
                    foreach (var item1 in item.PackingListDetailItem)
                    {
                        var transactionDetailModel = new WMSB_Transaction_Detail();
                        transactionDetailModel.Transac_No = Transac_No.Trim();
                        transactionDetailModel.Tool_Size = item1.Tool_Size;
                        transactionDetailModel.Order_Size = item1.Order_Size;
                        transactionDetailModel.Model_Size = item1.Model_Size;
                        transactionDetailModel.Spec_Size = item1.Spec_Size;
                        transactionDetailModel.Qty = item1.Received_Qty;
                        transactionDetailModel.Trans_Qty = item1.Received_Qty;
                        transactionDetailModel.Instock_Qty = item1.Received_Qty;
                        transactionDetailModel.Untransac_Qty = 0;
                        transactionDetailModel.Updated_Time = DateTime.Now;
                        transactionDetailModel.Updated_By = user;
                        _repoTransactionDetail.Add(transactionDetailModel);
                    }
                }
            }
            return await _repoTransactionDetail.SaveAll();
        }

        public async Task<string> CheckEnterRackInputIntergration(string racklocation, string receiveNo)
        {
            var packingListModel = await _repoPackingList.FindAll(x => x.Receive_No.Trim() == receiveNo.Trim()).FirstOrDefaultAsync();
            var supplier_ID = packingListModel.Supplier_ID.Trim();
            var rackModel = await _repoRacklocationRepo.FindAll(x => x.Rack_Location.Trim() == racklocation.Trim()).FirstOrDefaultAsync();
            var areaId = rackModel.Area_ID.Trim();
            if (supplier_ID == "V696")
            {
                if (areaId != "A012")
                {
                    return "input-rack-A012";
                }
                else
                {
                    return "ok";
                }
            }
            else
            {
                if (areaId == "A012")
                {
                    return "input-rack-not-A012";
                }
                else
                {
                    return "ok";
                }
            }
        }
    }
}