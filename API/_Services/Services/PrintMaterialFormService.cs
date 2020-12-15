using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.Models;
using Bottom_API.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class MaterialFormService : IMaterialFormService
    {
        private readonly IPackingListRepository _repoPackingList;
        private readonly IPackingListDetailRepository _repoPackingListDetail;
        private readonly IMaterialPurchaseRepository _repoMaterialPurchase;
        private readonly IMaterialViewRepository _repoMaterialView;
        private readonly IQRCodeMainRepository _repoQrcode;
        private readonly ITransactionMainRepo _repoTransactionMain;
        public MaterialFormService( IPackingListDetailRepository repoPackingListDetail,
                                    IPackingListRepository repoPackingList,
                                    IQRCodeMainRepository repoQrcode,
                                    IMaterialPurchaseRepository repoMaterialPurchase,
                                    ITransactionMainRepo repoTransactionMain,
                                    IMaterialViewRepository repoMaterialView) {
            _repoPackingListDetail = repoPackingListDetail;
            _repoPackingList = repoPackingList;
            _repoMaterialPurchase = repoMaterialPurchase;
            _repoQrcode = repoQrcode;
            _repoMaterialView = repoMaterialView;
            _repoTransactionMain = repoTransactionMain;
        }
        public async Task<List<QRCodeMainViewModel>> GetListQrCodeMainView(List<QrCodeIDVersion> data) {
            var listQrCode = data.Select(x => x.QRCode_ID).ToList();
            var listBatch = data.Select(x => x.MO_Seq).ToList();
            var packingList =  _repoPackingList.FindAll();
            var listMono = data.Select(x => x.MO_No.Trim()).ToList();
            var listQrCodeMain =  _repoQrcode
            .FindAll(x => listQrCode.Contains(x.QRCode_ID));
            var viewMaterialPurchase =  _repoMaterialView.FindAll(x => listBatch.Contains(x.MO_Seq) &&
                                                listMono.Contains(x.Plan_No.Trim()));
            var listQrCodeModel = await (from x in listQrCodeMain join y in packingList
                on x.Receive_No.Trim() equals y.Receive_No.Trim()
                join z in viewMaterialPurchase on
                new {Purchase_No =  y.Purchase_No.Trim(), PlanNo = y.MO_No.Trim()} equals new 
                {Purchase_No =  z.Purchase_No.Trim(), PlanNo = z.Plan_No.Trim()}
                select new QRCodeMainViewModel() {
                    QRCode_ID = x.QRCode_ID,
                    MO_No = y.MO_No,
                    Receive_No = x.Receive_No,
                    Receive_Date = y.Receive_Date,
                    Supplier_ID = y.Supplier_ID,
                    Supplier_Name = y.Supplier_Name,
                    T3_Supplier = y.T3_Supplier,
                    T3_Supplier_Name = y.T3_Supplier_Name,
                    Subcon_ID = y.Subcon_ID,
                    Subcon_Name = y.Subcon_Name,
                    Model_Name = y.Model_Name,
                    Model_No = y.Model_No,
                    Article = y.Article,
                    MO_Seq = z.MO_Seq,
                    Material_ID = y.Material_ID,
                    Material_Name = y.Material_Name,
                    Stockfiting_Date = z.Stockfiting_Date,
                    Assembly_Date = z.Assembly_Date,
                    CRD = z.CRD,
                    Line_ASY = z.Line_ASY,
                    Custmoer_Part = z.Custmoer_Part,
                    Custmoer_Name = z.Custmoer_Name
                }).ToListAsync();
            return listQrCodeModel;
        }
        
        
        // ---Phần in QrCodeId khi vào Menu 2.QrGenerate => Print (Material Form)-------//
        public async Task<object> FindByQrCodeID(QrCodeIDVersion data)
        {
            
            var qrCodeMan = _repoQrcode.FindSingle(x => x.QRCode_ID.Trim() == data.QRCode_ID.Trim() &&
                        x.QRCode_Version == data.QRCode_Version);
            
            // Tìm kiếm Purchase và Sheet_Type của qrcodeid với version đó.
            var packingListFind =  _repoPackingList.FindSingle(x => x.Receive_No.Trim() == qrCodeMan.Receive_No.Trim());
            // Tìm List ReceiveNo tương ứng với Purchase và Sheet_Type ở trên        
            var ReceiveNoList = await _repoPackingList.FindAll(x =>
                x.Purchase_No.Trim() == packingListFind.Purchase_No.Trim() &&
                x.Material_ID.Trim() == packingListFind.Material_ID.Trim() &&
                x.Sheet_Type.Trim() == packingListFind.Sheet_Type.Trim() &&
                x.MO_Seq == packingListFind.MO_Seq &&
                x.MO_No.Trim() == data.MO_No.Trim() &&
                x.Receive_Date <= packingListFind.Receive_Date)
                .Select(x => x.Receive_No).ToListAsync();
            
            var packingDetailList = new List<WMSB_PackingList_Detail>();
            foreach (var item in ReceiveNoList)
            {
                var packingdetailitem = await _repoPackingListDetail.FindAll(x => x.Receive_No.Trim() == item.Trim()).ToListAsync();
                packingDetailList.AddRange(packingdetailitem);
            }

            // Gộp theo từng tool size và tính tổng Purchase, Received Qty theo tool size đó.
            var BalByToolSize = packingDetailList.GroupBy(x => x.Tool_Size).Select(x => new {
                Tool_Size = x.FirstOrDefault().Tool_Size,
                Bal = x.Sum(cl => cl.Purchase_Qty)/(ReceiveNoList.Count()) - x.Sum(cl => cl.Received_Qty)
            });

            var packingListDetailModel = new List<PackingListDetailViewModel>();
            var packingListDetailModel1 = new List<PackingListDetailViewModel>();
            var packingListDetailModel2 = new List<PackingListDetailViewModel>();
            var packingListDetailModel3 = new List<PackingListDetailViewModel>();
            decimal? totalPQty = 0;
            decimal? totalRQty = 0;

            var lists = await _repoPackingListDetail.FindAll(x => x.Receive_No.Trim() == qrCodeMan.Receive_No.Trim()).ToListAsync();
            foreach (var item in lists)
            {
                    var packingItem1 = new PackingListDetailViewModel();
                    packingItem1.Receive_No = item.Receive_No;
                    packingItem1.Order_Size = item.Order_Size;
                    packingItem1.Model_Size = item.Model_Size;
                    packingItem1.Tool_Size = item.Tool_Size;
                    packingItem1.Spec_Size = item.Spec_Size;
                    packingItem1.MO_Qty = item.MO_Qty;
                    packingItem1.Purchase_Qty = item.Purchase_Qty;
                    packingItem1.Received_Qty = item.Received_Qty;
                    packingItem1.Act = 0;
                    foreach (var itemByToolSize in BalByToolSize)
                    {
                        if (itemByToolSize.Tool_Size.Trim() == item.Tool_Size.Trim()) {
                            packingItem1.Bal = itemByToolSize.Bal;
                        }
                    }
                    totalPQty = totalPQty + item.Purchase_Qty;
                    totalRQty = totalRQty + item.Received_Qty;
                    packingListDetailModel.Add(packingItem1);
            }

            //----------------- Xử lý mảng dữ liệu cho 1 số dòng cùng tool size.----------------//
              // List các Tool Size mà có nhiều Order Size trong bảng Packing List Detail
            var toolSizeMoreOrderSize = lists.Where(x => x.Tool_Size.Trim() != x.Order_Size.Trim()).Select(x => x.Tool_Size).Distinct().ToList();
            if(toolSizeMoreOrderSize.Count() > 0) {
                foreach (var itemToolSize in toolSizeMoreOrderSize)
                {
                    var model1 = packingListDetailModel.Where(x => x.Tool_Size.Trim() == itemToolSize.Trim())
                        .GroupBy(x => x.Tool_Size).Select(x => new {
                            Purchase_Qty = x.Sum(cl => cl.Purchase_Qty),
                            Received_Qty = x.Sum(cl => cl.Received_Qty),
                            Act = x.Sum(cl => cl.Act),
                            Bal = x.First().Bal
                        }).FirstOrDefault();
                    var packingDetailByToolSize = packingListDetailModel
                        .Where(x => x.Tool_Size.Trim() == itemToolSize.Trim()).ToList();
                    for (var i = 0; i < packingDetailByToolSize.Count; i ++)
                    {
                        if( i != 0) {
                                packingDetailByToolSize[i].Purchase_Qty = null;
                                packingDetailByToolSize[i].Received_Qty = null;
                                packingDetailByToolSize[i].Act = null;
                                packingDetailByToolSize[i].Bal = null;
                        } else {
                            packingDetailByToolSize[i].Purchase_Qty = model1.Purchase_Qty;
                            packingDetailByToolSize[i].Received_Qty = model1.Received_Qty;
                            packingDetailByToolSize[i].Act = model1.Act;
                            packingDetailByToolSize[i].Bal = model1.Bal;
                        }
                    }
                }
            }

            // Sắp xếp lại theo Tool-Size và Order_Size theo thứ tự tăng dần.
            packingListDetailModel = packingListDetailModel
            .OrderBy(x => decimal.Parse(x.Tool_Size)).ThenBy(x => decimal.Parse(x.Order_Size)).ToList();

            var count= packingListDetailModel.Count();
            if(count > 0 && count <=8) {
                packingListDetailModel1 = packingListDetailModel;
            } else if (count > 8 && count <= 16) {
                for (int i = 0; i < 8; i++)
                {
                    packingListDetailModel1.Add(packingListDetailModel[i]);
                }
                for (int i = 8; i < count; i++)
                {
                    packingListDetailModel2.Add(packingListDetailModel[i]);
                }
            } else if(count > 16) {
                for (int i = 0; i < 8; i++)
                {
                    packingListDetailModel1.Add(packingListDetailModel[i]);
                }
                for (int i = 8; i < 16; i++)
                {
                    packingListDetailModel2.Add(packingListDetailModel[i]);
                }
                for (int i = 16; i < count; i++)
                {
                    packingListDetailModel3.Add(packingListDetailModel[i]);
                }
            }

            var result = new {
                totalPQty,
                totalRQty,
                packingListDetailModel1, 
                packingListDetailModel2,
                packingListDetailModel3,
            };
                return result;
        }

        public async Task<List<object>> PrintByQRCodeIDList(List<QrCodeIDVersion> data)
        {
            var objectResult = new List<object>();
            var listAllQrCodeMainView = await this.GetListQrCodeMainView(data);
            foreach (var item in data)
            {   
                var qrCodeMainItem = listAllQrCodeMainView
                    .Where(x => x.QRCode_ID.Trim() == item.QRCode_ID.Trim() &&
                            x.MO_Seq == item.MO_Seq).FirstOrDefault();
                var object1 = await this.FindByQrCodeID(item);

                // Lấy dữ liệu show phần Suggested Location Material Form
                var transactionMain = await _repoTransactionMain
                        .FindAll(x => x.MO_No.Trim() == item.MO_No.Trim() &&
                            x.MO_Seq == item.MO_Seq &&
                            x.Can_Move.Trim() == "Y" &&
                            x.Material_ID.Trim() == qrCodeMainItem.Material_ID.Trim()).ToListAsync();
                            var suggestedReturn1 = transactionMain.Select(x => new {
                                x.Rack_Location
                            }).Distinct().ToList();
                
                var objectItem = new {
                    object1,
                    qrCodeMainItem,
                    suggestedReturn1
                };
                objectResult.Add(objectItem);
            }
            return objectResult;
        }

    }
}