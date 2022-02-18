import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SrmPoService } from '../../../business/srm/srm-po.service';
@Component({
  selector: 'app-po-examine',
  templateUrl: './po-examine.component.html',
  styleUrls: ['./po-examine.component.less']
})
export class PoExamineComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  data;
  status;

  page: number = 1;
  size: number = 6;
  total: number;
  t = "";
  constructor(private _formBuilder: FormBuilder,private _srmPoService: SrmPoService) { }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      PO_NUM: [null],
      STATUS: [0],
      EkgryDesc:[null]
    });
    this.refresh();
  }
  pageChange() {
    this.refresh();
  }
  submitSearch() {

    this.refresh();
  }
  refresh(){
    var query = {
      poNum: this.searchForm.value["PO_NUM"] == null ? "" : this.searchForm.value["PO_NUM"],
      status: this.searchForm.value["STATUS"] == null ? "" : this.searchForm.value["STATUS"],
      ekgryDesc: this.searchForm.value["EkgryDesc"] == null ? "" : this.searchForm.value["EkgryDesc"],
      page: this.page,
      size: this.size
    }
    console.log(this.searchForm);
    this._srmPoService.GetPoPoL(query).subscribe(result => {
      this.data = result["data"];
      this.total = result["count"];
      console.log(this.data);
    })

  }
}
