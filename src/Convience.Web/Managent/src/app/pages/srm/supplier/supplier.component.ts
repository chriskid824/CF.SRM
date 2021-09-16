import { Component, OnInit,ViewEncapsulation,ViewChild,TemplateRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StorageService } from '../../../services/storage.service';
import { SrmSupplierService } from '../../../business/srm/srm-supplier.service';
import { LayoutComponent } from '../../layout/layout/layout.component';
import { Router } from '@angular/router';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmModule } from '../srm.module';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Supplier } from '../../system-manage/model/supplier';

@Component({
  selector: 'app-supplier',
  templateUrl: './supplier.component.html',
  styleUrls: ['./supplier.component.less']
})
export class SupplierComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  editForm: FormGroup = new FormGroup({});
  data;
  status;
  defaultColDef;
  components;
  detailCellRendererParams;
  frameworkComponents: any;
  isedit:boolean;
  rowData: any;
  gridApi;
  tplModal: NzModalRef;

  @ViewChild('ctest1')
  ctest1: TemplateRef<any>;

  @ViewChild('ctest2')
  ctest2: TemplateRef<any>;


  page: number = 1;
  size: number = 6;
  total: number;
  



  editedSupplier: Supplier;
  isNewUser: boolean;




  constructor(
    private _formBuilder: FormBuilder,
    private _srmSrmService: SrmSupplierService,
    private _storageService: StorageService, 
    private _layout: LayoutComponent, 
    private _router: Router,
    private _modalService: NzModalService,
    private _messageService: NzMessageService) { 

    }
    



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

  addsupplier() {
    this._layout.navigateTo('supplier-c');
    this._router.navigate(['srm/supplier-c']);
    //window.open('../srm/rfq');
  }
  edit(title: TemplateRef<{}>, content: TemplateRef<{}>, supplier: Supplier) {

    console.log(supplier.srmVendor1+"，"+supplier.org+"，"+supplier.ekorg)
    var query = {
      code:supplier.srmVendor1,
      org:supplier.org,
      ekorg:supplier.ekorg
    }
    this._srmSrmService.GetSupplierDetail(query).subscribe(result => {

      console.log(result);
      console.log(result['srmVendor1']);
      this.isNewUser = false;
      this.editedSupplier = result;
      this.editForm = this._formBuilder.group({
               
        srmvendor: [result['srmVendor1']],
        vendorname: [result['vendorName']],
        org: [result['org']],
        ekorg: [result['ekorg']],
        person: [result['person']],
        address: [result['address']],
        telphone: [result['telPhone']],
        ext: [result['ext']],
        faxnumber: [result['faxNumber']],
        cellphone: [result['cellPhone']],
        mail: [result['mail']],
        status: [result['statusDesc']],
      });
      this.tplModal = this._modalService.create({
        nzTitle: title,
        nzContent: content,
        nzFooter: null,
        nzClosable: true,
        nzMaskClosable: false
      });
    });
  }

  submitEdit() {
    for (const i in this.editForm.controls) {
      this.editForm.controls[i].markAsDirty();
      this.editForm.controls[i].updateValueAndValidity();
    }
    if (this.editForm.valid) {
      let supplier: any = {};
      supplier.srmVendor1 = this.editForm.value['srmvendor'];
      supplier.vendorname = this.editForm.value['vendorname'];
      supplier.org = this.editForm.value['org'];
      supplier.ekorg = this.editForm.value['ekorg'];
      supplier.person = this.editForm.value['person'];
      supplier.address=this.editForm.value['address'];
      supplier.telphone = this.editForm.value['telphone'];
      supplier.ext = this.editForm.value['ext'];
      supplier.faxnumber = this.editForm.value['faxnumber'];
      supplier.cellphone = this.editForm.value['cellphone'];
      supplier.mail = this.editForm.value['mail'];
      supplier.statusdesc = this.editForm.value['status'];
      supplier.user = this._storageService.userName;
      console.log(supplier.status);

      this._srmSrmService.update(supplier).subscribe(result => {
        this._messageService.success("更新成功！");
        this.tplModal.close();
        this.refresh();
      });
    }
  }


  cancelEdit() {
    this.tplModal.close();
  }

  refresh() {
    var query = {
      code: this.searchForm.value["SRM_VENDOR"] == null ? "" : this.searchForm.value["SRM_VENDOR"],
      vendor: this.searchForm.value["VENDOR_NAME"] == null ? "" : this.searchForm.value["VENDOR_NAME"],
      Werks: this._storageService.werks.split(','),
      page: this.page,
      size: this.size
    }
    //console.log(query);
    this._srmSrmService.GetSupplierList(query).subscribe(result => {
      this.data = result["data"];
      this.total = result["count"];
    });
  }
}
 
    
