import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { FunctionUtility } from '../_utility/function-utility';
import { BehaviorSubject, Observable } from 'rxjs';
import { ReportMaterialReceive } from '../_viewmodels/report-material-receive';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  baseUrl = environment.apiUrl;
  kanbanByPoSource = new BehaviorSubject<any>([]);
  currentKanbanByPo = this.kanbanByPoSource.asObservable();
  paramSearchSource = new BehaviorSubject<any>({
    dateType: 1,
    dateStart: new Date(),
    dateEnd: new Date(),
    moNo: '',
    batch: '',
    supplier: '',
    currenPage: 1
  });
  currentParamSearch = this.paramSearchSource.asObservable();

  constructor(private http: HttpClient, private functionUtility: FunctionUtility, private spiner: NgxSpinnerService) { }
  getMaterialReceive(dateType, dateStart, dateEnd, moNo, batch, supplier, status, article): Observable<any[]> {
    const reportMatRecParam = new ReportMaterialReceive();
    reportMatRecParam.dateType = parseInt(dateType);
    reportMatRecParam.moNo = moNo;
    reportMatRecParam.moSeq = batch;
    reportMatRecParam.supplier = supplier;
    reportMatRecParam.status = status;
    reportMatRecParam.article = article;
    reportMatRecParam.dateStart = dateType == 5 ? "" : dateStart;
    reportMatRecParam.dateEnd = dateType == 5 ? "" : dateEnd;
    return this.http.post<any[]>(this.baseUrl + "Report/getMaterial", reportMatRecParam);
  }
  exportExcelMaterialReceive(dateType, dateStart, dateEnd, moNo, batch, supplier, status, article) {
    const reportMatRecParam = new ReportMaterialReceive();
    reportMatRecParam.dateType = parseInt(dateType);
    reportMatRecParam.moNo = moNo;
    reportMatRecParam.moSeq = batch;
    reportMatRecParam.supplier = supplier;
    reportMatRecParam.status = status;
    reportMatRecParam.article = article;
    reportMatRecParam.dateStart = dateType == 5 ? '' : dateStart;
    reportMatRecParam.dateEnd = dateType == 5 ? '' : dateEnd;

    return this.http.post(this.baseUrl + 'Report/exportmatrec', reportMatRecParam, { responseType: 'blob' })
      .subscribe((result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_MaterialReceived_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
        this.spiner.hide();
      });
  }
}
