import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OutputPrintQrCode } from '../../../_core/_models/output-print-qrcode';
import { OutputService } from '../../../_core/_services/output.service';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { Location } from '@angular/common';
import { InputService } from '../../../_core/_services/input.service';

@Component({
  selector: 'app-output-print-qrcode-again',
  templateUrl: './output-print-qrcode-again.component.html',
  styleUrls: ['./output-print-qrcode-again.component.scss']
})
export class OutputPrintQrcodeAgainComponent implements OnInit {
  elementType: 'url' | 'canvas' | 'img' = 'url';
  listOutputPrintQrCode: OutputPrintQrCode[] = [];
  backUrl: string = '';
  paramPrintQrCodeAgain: any = [];
  checkPrintLocation = localStorage.getItem('checkPrintLocation');
  checkPrintLocationIntergation = localStorage.getItem('checkPrintLocationIntergation');

  constructor(private router: Router,
              private inputService: InputService, 
              private outputService: OutputService, 
              private alertifyService: AlertifyService,
              private spinnerService: NgxSpinnerService,
              private locationBack: Location) { }

  ngOnInit() {
    this.outputService.currentPrintQrCode.subscribe(res => this.backUrl = res).unsubscribe();
    this.outputService.currentParamPrintQrCodeAgain.subscribe(res => this.paramPrintQrCodeAgain = res).unsubscribe();
    this.getData();
  }

  back() {
    if (this.backUrl === '1') {
      this.router.navigate(['/input/qrcode-again']);
    } else if (this.backUrl === '2') {
      this.router.navigate(['/output/main']);
    } else if(this.backUrl === '3'){
      this.inputService.changeIsSubmitIntegration(true);
      this.router.navigate(['/qr/integration']);
    } else {
      this.locationBack.back();
    }
  }

  getData() {
    this.spinnerService.show();
    this.outputService.printQrCode(this.paramPrintQrCodeAgain).subscribe(res => {
      this.listOutputPrintQrCode = res;
      this.listOutputPrintQrCode.forEach(element => {
        if (element.packingListDetail.length <= 8) {
          element.print1 = element.packingListDetail;
          element.print2 = [];
          element.print3 = [];
          element.print4 = [];
        }
        else if (element.packingListDetail.length > 8 && element.packingListDetail.length <= 16) {
          element.print1 = element.packingListDetail.slice(0, 8);
          element.print2 = element.packingListDetail.slice(8, element.packingListDetail.length);
          element.print3 = [];
          element.print4 = [];
        }
        else if (element.packingListDetail.length > 16 && element.packingListDetail.length <= 24) {
          element.print1 = element.packingListDetail.slice(0, 8);
          element.print2 = element.packingListDetail.slice(8, 16);
          element.print3 = element.packingListDetail.slice(16, element.packingListDetail.length);
          element.print4 = [];
        }
        else {
          element.print1 = element.packingListDetail.slice(0, 8);
          element.print2 = element.packingListDetail.slice(8, 16);
          element.print3 = element.packingListDetail.slice(16, 24);
          element.print4 = element.packingListDetail.slice(24, element.packingListDetail.length);
        }
        element.totalPQty = element.packingListDetail.reduce((value, i) => {
          return value += i.purchase_Qty;
        }, 0);
        element.totalRQty = element.packingListDetail.reduce((value, i) => {
          return value += i.act;
        }, 0);
      });

      this.spinnerService.hide();
    }, error => {
      this.spinnerService.hide();
      this.alertifyService.error('error');
    });
  }
  trackByFn(index: number, item: OutputPrintQrCode): string {
    return item.qrCodeModel.qrCode_ID;
  }
}
