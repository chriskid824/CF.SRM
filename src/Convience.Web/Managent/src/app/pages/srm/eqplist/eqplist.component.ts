import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StorageService } from '../../../services/storage.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LayoutComponent } from '../../layout/layout/layout.component';
import { SrmEqpService } from 'src/app/business/srm/srm-eqp.service';

@Component({
  selector: 'app-eqplist',
  templateUrl: './eqplist.component.html',
  styleUrls: ['./eqplist.component.less']
})
export class EqplistComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  data;
  status;

  page: number = 1;
  size: number = 6;
  total: number;
  t = "";

  _eqpNum;
  _woNum;
  _no;
  _matnr;
  _status;
  _vendeor;

  constructor(
    private activatedRoute: ActivatedRoute,
    private _formBuilder: FormBuilder,
    private _srmEqpService: SrmEqpService,
    private _storageService: StorageService,
    private _router: Router,
    private _layout: LayoutComponent) {

  }


  selectedValue = null
  ngOnInit(): void {

    this.searchForm = this._formBuilder.group({
      EQP_NUM: [null],
      woNum: [null],
      no: [null],
      matnr: [null],
      STATUS: ["1"],
    });
    this.refresh();
    if (sessionStorage.getItem("eqplist")) {
      var query = JSON.parse(sessionStorage.getItem("eqplist"));
      this._eqpNum = query.eqpNum;
      this._woNum = query.woNum;
      this._no = query.no;
      this._matnr = query.matnr;
      this.page = query.page;
      this.size = query.size;
      this._status = query.status;
      this._vendeor = query.vendor;

      console.log(this._status);
      this.searchForm.patchValue({
        eqpNum: this._eqpNum,
        no: this._no,
        woNum: this._woNum,
        matnr: this._matnr,
        STATUS: this._status,
        vendor: this._vendeor
      });
      console.log(this.searchForm);
      this.refresh();
    }
    if(this._vendeor != null)
    {
     console.log('ngOnInit')
      this.refresh();
    }
  }

  submitSearch() {
    this._eqpNum = this.searchForm.value["EQP_NUM"] == null ? "" : this.searchForm.value["EQP_NUM"];
    this._woNum = this.searchForm.value["woNum"] == null ? "" : this.searchForm.value["woNum"];
    this._no = this.searchForm.value["no"] == null ? "" : this.searchForm.value["no"];
    this._matnr = this.searchForm.value["matnr"] == null ? "" : this.searchForm.value["matnr"];
    this._status = this.searchForm.value["STATUS"] == null ? "" : this.searchForm.value["STATUS"];
    this._vendeor =this._storageService.userName;
    this.page = 1;
    this.refresh();
  }

  refresh() {
    console.log(this.searchForm.value["STATUS"]);
    var query = {
      eqpNum: this._eqpNum,
      woNum: this._woNum,
      no: this._no,
      matnr: this._matnr,
      page: this.page,
      size: this.size,
      status: this._status,
      vendor:this._storageService.userName

    }
    //alert(this._storageService.userName)
    //alert(query.vendor)
    console.log(query);
    this._srmEqpService.GetEqpList(query).subscribe(result => {
      this.data = result["data"];
      this.total = result["count"];
      console.log('GetEqpList------');
      console.log(this.data);
    })
    this.status = query.status;
    sessionStorage.setItem("eqplist", JSON.stringify(query));
  }

  pageChange() {
    this.refresh();
  }
  sizeChange() {
    this.page = 1;
    this.refresh();
  }
  open(id) {
    this._layout.navigateTo('eqp');
    this._router.navigate(['srm/eqp', { id: id }]);
  }
  GoToEQP()
  {
    window.open('../srm/eqp', "_self");
  }
}
