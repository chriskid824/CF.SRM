import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray, FormControl} from '@angular/forms';
import { StorageService } from '../../../services/storage.service';
import { SrmSupplierService } from '../../../business/srm/srm-supplier.service';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-supplier-c',
  templateUrl: './supplier-c.component.html',
  styleUrls: ['./supplier-c.component.less']
})
export class SupplierCComponent implements OnInit {
  formDetail: FormGroup = new FormGroup({});
  searchForm: FormGroup = new FormGroup({});
  werks = [];

  constructor(
    private _storageService: StorageService, 
    private _formBuilder: FormBuilder,
    private _srmSrmService: SrmSupplierService,
    private _messageService: NzMessageService) { }

  ngOnInit(): void { 
    this.initRoleList();    

    this.searchForm = this._formBuilder.group({
      sap_vendor:[null],
      srm_vendor: [null,[Validators.required]],
      vendorname: [null,[Validators.required]],
      companyid: [null,[Validators.required]],
      werks: [null,[Validators.required]],
      person: [null,[Validators.required]],
      address: [null,[Validators.required]],
      telphone: [null],
      ext: [null],
      faxnumber: [null],
      cellphone: [null],
      email: [null,[Validators.required]],
      //roles: this._storageService.werks.split(','),
    });
  }
  initRoleList() {    
    this.werks = this._storageService.werks.split(',');
    console.log(this.werks );
  }

  add(){
    var query = {
      srmVendor1: this.searchForm.value["srm_vendor"] == null ? "" : this.searchForm.value["srm_vendor"],
    }
    if (!this.searchForm.value["srm_vendor"]) {
      alert("請輸入SRM供應商代碼");
      return;
    }
    if (!this.searchForm.value["werks"]) {
      alert("請選擇採購組織");
      return;
    }
    //console.log(query);
    this._srmSrmService.CheckSupplier(query).subscribe(result => {
      for (const i in this.searchForm.controls) {
        console.log(i);
        if(i!="sap_vendor" && i!="telphone" && i!="ext" && i!="faxnumber" && i!="cellphone")
        {
          this.searchForm.controls[i].markAsDirty();
          this.searchForm.controls[i].updateValueAndValidity();
        }
      }
      $('#addBar').show();
      var supplier ={
        srmVendor1 : this.searchForm.value['srm_vendor'],
        sapVendor : this.searchForm.value['sap_vendor'],
        vendorName : this.searchForm.value['vendorname'],
        org : this.searchForm.value['companyid'],
        ekorg : this.searchForm.value['werks'],
        person : this.searchForm.value['person'],
        address : this.searchForm.value['address'],
        telphone : this.searchForm.value['telphone'],
        ext : this.searchForm.value['ext'],
        faxnumber : this.searchForm.value['faxnumber'],
        cellphone : this.searchForm.value['cellphone'],
        mail : this.searchForm.value['email'],
        user : this._storageService.userName,     
      }
      console.log(supplier);
      if (this.searchForm.valid)
      {        
        this._srmSrmService.AddSupplier(supplier).subscribe(result => {
          this._messageService.success("供應商建立成功！");
        });
      }

    });
  }

}
