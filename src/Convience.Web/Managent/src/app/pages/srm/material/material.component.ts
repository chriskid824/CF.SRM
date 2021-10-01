import { Component, OnInit,ViewEncapsulation,ViewChild,TemplateRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StorageService } from '../../../services/storage.service';
import { LayoutComponent } from '../../layout/layout/layout.component';
import { Router } from '@angular/router';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmModule } from '../srm.module';
import { NzMessageService } from 'ng-zorro-antd/message';
import { SrmMaterialService } from '../../../business/srm/srm-material.service';
import { Material } from '../../system-manage/model/material';
import { User } from '../../system-manage/model/user';

@Component({
  selector: 'app-material',
  templateUrl: './material.component.html',
  styleUrls: ['./material.component.less']
})
export class MaterialComponent implements OnInit {

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
  



  editedMaterial: Material;
  isNewUser: boolean;




  constructor(
    private _formBuilder: FormBuilder,
    //private _srmSrmService: SrmSupplierService,
    private _storageService: StorageService, 
    private _layout: LayoutComponent, 
    private _router: Router,
    private _modalService: NzModalService,
    private _messageService: NzMessageService,
    private _srmMaterialService: SrmMaterialService,) { 

    }
    



  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      material: [null],
      name:[null]
    });
  }

  submitSearch() {
    this.refresh();
  }
  pageChange() {
    this.refresh();
  }

  addmaterial() {
    //this._layout.navigateTo('material-c');
    this._router.navigate(['srm/material-c']);
    //window.open('../srm/rfq');
  }
  edit(title: TemplateRef<{}>, content: TemplateRef<{}>, material: Material) {

    //console.log(material.srmMatnr1)
    var query = {
      material:material.srmMatnr1
    }
    this._srmMaterialService.GetMaterialDetail(query).subscribe(result => {      

      //console.log(result);

      this.isNewUser = false;
      this.editedMaterial = result;
      this.editForm = this._formBuilder.group({
               
        srmMatnr1: [result['srmMatnr1']],
        matnrGroup: [result['matnrGroup']],
        description: [result['description']],
        version: [result['version']],
        material: [result['material']],
        length: [result['length']],
        width: [result['width']],
        height: [result['height']],
        density: [result['density']],
        weight: [result['weight']],
        gewei: [result['gewei']],
        ekgrp: [result['ekgrp']],
        statusDesc: [result['statusDesc']],
        note: [result['note']],
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
      let material: any = {};
      material.srmMatnr1 = this.editForm.value['srmMatnr1'];
      material.matnrGroup = this.editForm.value['matnrGroup'];
      material.description = this.editForm.value['description'];
      material.version = this.editForm.value['version'];
      material.material = this.editForm.value['material'];
      material.length=this.editForm.value['length'];
      material.width = this.editForm.value['width'];
      material.height = this.editForm.value['height'];
      material.density = this.editForm.value['density'];
      material.weight = this.editForm.value['weight'];
      material.statusDesc = this.editForm.value['statusDesc'];
      material.note = this.editForm.value['note'];
      material.user = this._storageService.userName;
      material.gewei = this.editForm.value['gewei'];
      material.ekgrp = this.editForm.value['ekgrp'];
      console.log(this._storageService.userName);

      this._srmMaterialService.update(material).subscribe(result => {
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
      material: this.searchForm.value["material"] == null ? "" : this.searchForm.value["material"],
      name: this.searchForm.value["name"] == null ? "" : this.searchForm.value["name"],
      Werks: this._storageService.werks.split(','),
      page: this.page,
      size: this.size
    }
    //console.log(query);
    this._srmMaterialService.GetMaterialList(query).subscribe(result => {
      this.data = result["data"];
      this.total = result["count"];
    });
  }
}

 
    
