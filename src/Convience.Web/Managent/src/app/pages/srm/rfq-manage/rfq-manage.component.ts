import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SrmRfqService } from '../../../business/srm/srm-rfq.service';
import { Rfq } from '../model/rfq';
import { StorageService } from '../../../services/storage.service';
import { Router } from '@angular/router';
import { LayoutComponent } from '../../layout/layout/layout.component';

@Component({
  selector: 'app-rfq-manage',
  templateUrl: './rfq-manage.component.html',
  styleUrls: ['./rfq-manage.component.less']
})
export class RfqManageComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  data;
  status;

  page: number = 1;
  size: number = 6;
  total: number;
  t = "";
  constructor(private _formBuilder: FormBuilder,
    private _srmRfqService: SrmRfqService,
    private _storageService: StorageService,
    private _router: Router,
    private _layout: LayoutComponent  ) { }


  selectedValue = null

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      RFQ_NUM: [null],
      STATUS: [1],
      NAME:[null]
    });
  }

  submitSearch() {
    this.page = 1;
    this.refresh();
  }

  test(): string {
    var d = new Date(),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

    if (month.length < 2)
      month = '0' + month;
    if (day.length < 2)
      day = '0' + day;

    return [year, month, day].join('-');
  };

  refresh() {
    //this.t = this.test();
    console.log(this.searchForm.value["STATUS"]);
    console.log(this._storageService.werks);
    var query = {
      rfqNum: this.searchForm.value["RFQ_NUM"] == null ? "" : this.searchForm.value["RFQ_NUM"],
      status: this.searchForm.value["STATUS"] == null ? "" : this.searchForm.value["STATUS"],
      name: this.searchForm.value["NAME"] == null ? "" : this.searchForm.value["NAME"],
      werks: this._storageService.werks,
      page: this.page,
      size: this.size
    }
    console.log(query);
    this._srmRfqService.GetRfqList(query).subscribe(result => {
      this.data = result["data"];
      this.total = result["count"];
      console.log(this.data);
    })
    this.status = query.status;
    //this.data = [
    //  { RFQ_ID: 1, RFQ_NUM: 'order 1', STATUS: 1, SOURCER: 'TEST', DateString: this.t,CREATE_BY:'TEST'},
    //  { RFQ_ID: 2, RFQ_NUM: 'order 2', STATUS: 1, SOURCER: 'TEST', DateString: this.t,CREATE_BY:'TEST' },
    //  { RFQ_ID: 3, RFQ_NUM: 'order 3', STATUS: 1, SOURCER: 'TEST', DateString: this.t,CREATE_BY:'TEST' },
    //  { RFQ_ID: 4, RFQ_NUM: 'order 4', STATUS: 1, SOURCER: 'TEST', DateString: this.t,CREATE_BY:'TEST' }
    //];
  }

  // reset the search form content 
  resetSearchForm() {
    this.searchForm = this._formBuilder.group({
      userName: [null],
      phoneNumber: [null],
      name: [null],
      roleid: [null],
      position: [null]
    });
  }


  pageChange() {
    this.refresh();
  }

  sizeChange() {
    this.page = 1;
    this.refresh();
  }

  open(id) {
    this._layout.navigateTo('rfq');
    this._router.navigate(['srm/rfq', { id: id }]);
    //window.open('../srm/rfq?id=' + id);
  }
  openPrice(id) {
    this._layout.navigateTo('price');
    this._router.navigate(['srm/price', { id: id  }]);
    //window.open('../srm/rfq?id=' + id);
  }
  addRfq() {
    this._layout.navigateTo('rfq');
    this._router.navigate(['srm/rfq']);
    //window.open('../srm/rfq');
  }
}
