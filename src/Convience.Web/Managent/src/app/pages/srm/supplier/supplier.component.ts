import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StorageService } from '../../../services/storage.service';
import { SrmRfqService } from '../../../business/srm/srm-rfq.service';

@Component({
  selector: 'app-supplier',
  templateUrl: './supplier.component.html',
  styleUrls: ['./supplier.component.less']
})
export class SupplierComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  data;
  status;

  page: number = 1;
  size: number = 6;
  total: number;


  constructor(
    private _formBuilder: FormBuilder,
    private _srmSrmRfqService: SrmRfqService,
    private _storageService: StorageService ) { }


  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      SRM_VENDOR: [null],
      VENDOR_NAME:[null]
    });
  }

  submitSearch() {
    this.refresh();
  }
  pageChange() {
    this.refresh();
  }

  refresh() {
    var query = {
      code: this.searchForm.value["SRM_VENDOR"] == null ? "" : this.searchForm.value["SRM_VENDOR"],
      name: this.searchForm.value["VENDOR_NAME"] == null ? "" : this.searchForm.value["VENDOR_NAME"],
      Werks: this._storageService.werks.split(','),
      page: this.page,
      size: this.size
    }
    console.log(query);
    this._srmSrmRfqService.GetSupplierList(query).subscribe(result => {
      this.data = result["data"];
    })
    //this.status = query.status;
  }

}
