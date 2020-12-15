import { Component, OnInit, Pipe, PipeTransform } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { TransferFormGenerate } from '../../../_core/_models/transfer-form-generate';
import { TransferFormPrint, TransferFormPrintQty } from '../../../_core/_models/transfer-form-print';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { TransferFormService } from '../../../_core/_services/transfer-form.service';

@Pipe({
  name: 'sum'
})
export class SumPipe implements PipeTransform {
  transform(items: TransferFormPrintQty[], attr: string): any {
      return items.reduce((sum, item) => sum + item[attr], 0);
  }
}
@Component({
  selector: 'app-print-form',
  templateUrl: './print-form.component.html',
  styleUrls: ['./print-form.component.scss']
})
export class PrintFormComponent implements OnInit {
  transferFormPrintDetail: TransferFormPrint[] = [];
  transferFormPrint: TransferFormGenerate[];
  today: Date = new Date();
  elementType: 'url' | 'canvas' | 'img' = 'url';

  constructor(private transferFormService: TransferFormService,
    private spinnerService: NgxSpinnerService,
    private alertifyService: AlertifyService) { }

  ngOnInit(): void {
    this.transferFormService.currentTransferFormPrintDetail.subscribe(res => {
      this.transferFormPrint = res;
    });
    this.getData();
  }

  getData() {
    this.spinnerService.show();
    this.transferFormService.getInfoTransferFormPrintDetail(this.transferFormPrint).subscribe(res => {
      this.transferFormPrintDetail = res;
      this.spinnerService.hide();
    }, error => {
      this.spinnerService.hide();
      this.alertifyService.error('Error Server');
      console.log(error);
    });
  }

}
