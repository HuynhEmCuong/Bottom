import { Component, OnInit } from '@angular/core';
import { TransactionMain } from '../../../_core/_models/transaction-main';
import { InputService } from '../../../_core/_services/input.service';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { Pagination, PaginatedResult } from '../../../_core/_models/pagination';
import { FilterMissingParam } from '../../../_core/_viewmodels/missing-print-search';
import { NgxSpinnerService } from 'ngx-spinner';
import { FunctionUtility } from '../../../_core/_utility/function-utility';

@Component({
  selector: 'app-missing-again',
  templateUrl: './missing-again.component.html',
  styleUrls: ['./missing-again.component.scss']
})
export class MissingAgainComponent implements OnInit {
  transactionMainList: TransactionMain[] = [];
  mO_No: string;
  material_ID: string;
  material_Name: string;
  pagination: Pagination;
  missingParam: FilterMissingParam;
  alerts: any = [
    {
      type: 'success',
      msg: `You successfully read this important alert message.`
    },
    {
      type: 'info',
      msg: `This alert needs your attention, but it's not super important.`
    },
    {
      type: 'danger',
      msg: `Better check yourself, you're not looking too good.`
    }
  ];
  constructor(private inputService: InputService,
              private functionUtility: FunctionUtility,
              private spinnerService: NgxSpinnerService,
              private alertifyService: AlertifyService) { }

  ngOnInit() {
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 5,
      totalItems: 0,
      totalPages: 0
    };
    this.inputService.currentMissingParam.subscribe(res => this.missingParam = res);
    console.log(this.missingParam);
    if (this.missingParam !== undefined && this.missingParam !== null) {
      this.mO_No = this.missingParam.mO_No;
      this.material_ID = this.missingParam.material_ID;
      if(this.material_ID !== undefined) {
        this.findMaterialName();
      }
      this.search();
    }
    this.inputService.clearDataChangeMenu();
  }
  getData() {
    this.spinnerService.show();
    this.inputService.missingPrintFilter(this.pagination.currentPage , this.pagination.itemsPerPage,this.missingParam)
    .subscribe((res: PaginatedResult<TransactionMain[]>) => {
      this.transactionMainList = res.result;
      this.pagination = res.pagination;
      this.spinnerService.hide();
      if(this.transactionMainList.length === 0) {
        this.alertifyService.error('No Data!');
      }
    }, error => {
      this.alertifyService.error(error);
    });
  }
  findMaterialName() {
    if(!(this.functionUtility.checkEmpty(this.material_ID))) {
      this.inputService.findMaterialName(this.material_ID).subscribe(res => {
        this.material_Name = res.materialName;
      });
    } else {
      this.material_Name = '';
    }
  }
  search(){
    this.pagination.currentPage = 1;
    this.missingParam = {
      mO_No: this.mO_No,
      material_ID: this.material_ID
    }
    this.inputService.changeMissingParam(this.missingParam);
    this.getData();
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.getData();
  }
}
