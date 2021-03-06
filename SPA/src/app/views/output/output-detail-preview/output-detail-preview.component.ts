import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OutputService } from '../../../_core/_services/output.service';

@Component({
  selector: 'app-output-detail-preview',
  templateUrl: './output-detail-preview.component.html',
  styleUrls: ['./output-detail-preview.component.scss']
})
export class OutputDetailPreviewComponent implements OnInit {
  outputDetail: any = [];
  totalTransOutputQty: number;
  output: any = {
    qrCodeId: '',
    matId: '',
    matName: '',
    batch: '',
    planNo: ''
  };
  transactionDetail: any = [];
  result1 = [];
  index: number = 0;

  constructor(
    private router: Router,
    private outputService: OutputService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.outputDetail.transactionDetail = [];
    this.index = this.route.snapshot.params['index'];
    this.outputService.currentListOutputSave.subscribe(res => {
      if (res.length !== 0) {
        this.outputDetail = res[this.index];
        this.output = res[this.index].output;
        this.transactionDetail = res[this.index].transactionDetail;
        this.totalTransOutputQty = this.transactionDetail.map(x => x.trans_Qty).reduce((a,c) => {return a + c});
      }
    }).unsubscribe();
  }

  back() {
    this.router.navigate(['output/main']);
  }
  getData() {
    // Group by theo tool_Size
    const groups = new Set(this.outputDetail.transactionDetail.map((item) => item.tool_Size)), results = [];
    groups.forEach((g) =>
      results.push({
        name: g,
        value: this.outputDetail.transactionDetail.filter((i) => i.tool_Size === g).reduce((trans_Qty, j) => {
          return trans_Qty += j.trans_Qty;
        }, 0),
        colspan: this.outputDetail.transactionDetail.filter((i) => i.tool_Size === g).length
      })
    );
    this.result1 = results;
  }

}
