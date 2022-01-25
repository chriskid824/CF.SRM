import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute,Router } from '@angular/router';
import { LayoutComponent } from '../../layout/layout/layout.component';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmRfqService } from '../../../business/srm/srm-rfq.service';
import { StorageService } from '../../../services/storage.service';
@Component({
  selector: 'app-price-work',
  templateUrl: './price-work.component.html',
  styleUrls: ['./price-work.component.less']
})
export class PriceWorkComponent implements OnInit {
  matnrList: FormGroup = new FormGroup({});
  form_searchRFQ: FormGroup = new FormGroup({});
  selectedMatnr;
  matnrs;
  rfqId;

  constructor(private _formBuilder: FormBuilder
    , private _router: Router
    , private _layout: LayoutComponent
    , private _srmRfqService: SrmRfqService
    , private _storageService: StorageService
    ,private activatedRoute: ActivatedRoute
  ) {
    this.activatedRoute.params.subscribe(params => {
      this.rfqId = params['id'];
      console.log(params['id']);
      console.log("RFQID:" + this.rfqId); // Print the parameter to the console.
    });
  }

  ngOnInit(): void {
    this.form_searchRFQ = this._formBuilder.group({
      rfqNum: [null],
    });
    if(this.rfqId!=undefined)
    {
      this.searchRFQ();
    }
  }
  searchRFQ() {
    if (!this.form_searchRFQ.value["rfqNum"]&&this.rfqId==undefined) {
      alert("詢價單號必填");
      return;
    }
    this._srmRfqService.GetRfq({ rfqId:this.rfqId,rfqNum: this.form_searchRFQ.value["rfqNum"], statuses: [5, 7, 8, 18]}).subscribe(result => {
      /*      console.log(result);*/
      if (result) {
        this._layout.navigateTo('price');
        this._router.navigate(['srm/price', { id: result["rfqId"] }]);
      } else {
        alert("查無詢價單或尚未完成報價");
      }
    });
  }
}
