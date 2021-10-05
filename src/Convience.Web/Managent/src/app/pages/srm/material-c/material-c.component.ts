import { NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators,FormArray, FormControl} from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { SrmMaterialService } from '../../../business/srm/srm-material.service';
import { StorageService } from '../../../services/storage.service';
import { Role } from '../../system-manage/model/role';
import { RoleService } from 'src/app/business/system-manage/role.service';
import { NullLogger } from '@microsoft/signalr';

@Component({
  selector: 'app-material-c',
  templateUrl: './material-c.component.html',
  styleUrls: ['./material-c.component.less']
})
export class MaterialCComponent implements OnInit {
  formDetail: FormGroup = new FormGroup({});
  searchForm: FormGroup = new FormGroup({});
  editForm: FormGroup = new FormGroup({});
  werks = [];


 

  constructor(
    private _formBuilder: FormBuilder,
    private _messageService: NzMessageService,
    private _srmMaterialService: SrmMaterialService,
    private _storageService: StorageService, 
    private _roleService: RoleService,) { 
           
    }

    

  ngOnInit(): void {
    this.initRoleList();

    
    //console.log(this.matnr);


    this.searchForm = this._formBuilder.group({
      sap_matnr:[null],
      matnr: [null,[Validators.required]],
      werks: [null,[Validators.required]],
      group: [null,[Validators.required]],
      description: [null,[Validators.required]],
      version: [null,[Validators.required]],
      material: [null,[Validators.required]],
      length: [null,[Validators.required]],
      width: [null,[Validators.required]],
      height: [null,[Validators.required]],
      density: [null,[Validators.required]],
      weight: [null,[Validators.required]],
      note: [null],
      ekgrp: [null,[Validators.required]],
      gewei: [null,[Validators.required]],
      bn_num: [null],
      //roles: this._storageService.werks.split(','),
    });
    this.getsrmmatnr();
  }

  initRoleList() {    
    this.werks = this._storageService.werks.split(',');
    console.log(this.werks );
  }
  getsrmmatnr(){
    var query = {
      matnr: this.searchForm.value["matnr"] == null ? "" : this.searchForm.value["matnr"],
    } 
    this._srmMaterialService.GetMatnr(query).subscribe(result =>{
      console.log(result['srmMatnr1']);
      this.searchForm.controls['matnr'].setValue(result['srmMatnr1']);

    });
  }


  add() {
    //console.log(this.matnr);
    //$('#addBar').hide();
    var query = {
      material: this.searchForm.value["matnr"] == null ? "" : this.searchForm.value["matnr"],
      sapMatnr: this.searchForm.value["sap_matnr"] == null ? "" : this.searchForm.value["sap_matnr"],
    }
    if (!this.searchForm.value["matnr"]) {
      alert("請輸入料號");
      return;
    }
    if (!this.searchForm.value["werks"]) {
      alert("請選擇廠別");
      return;
    }
    //console.log(query);
    if(this.searchForm.valid)
    {
      //alert(111);
      this._srmMaterialService.CheckMatnr(query).subscribe(result => {
        this._srmMaterialService.CheckSAPMatnr(query).subscribe(result => {

          $('#addBar').show();
          var material ={
            srmMatnr1 : this.searchForm.value['matnr'],
            sapMatnr : this.searchForm.value['sap_matnr'],
            matnrGroup : this.searchForm.value['group'],
            description : this.searchForm.value['description'],
            version : this.searchForm.value['version'],
            material : this.searchForm.value['material'],
            length : this.searchForm.value['length'],
            width : this.searchForm.value['width'],
            height : this.searchForm.value['height'],
            density : this.searchForm.value['density'],
            weight : this.searchForm.value['weight'],
            note : this.searchForm.value['note'],
            user : this._storageService.userName,     
            werks : this.searchForm.value['werks'],
            gewei : this.searchForm.value['gewei'],
            ekgrp : this.searchForm.value['ekgrp'],
            bn_num : this.searchForm.value['bn_num'],
          }
          console.log(material);
          if (this.searchForm.valid)
          {        
            this._srmMaterialService.AddMatnr(material).subscribe(result => {
              this._messageService.success("料號建立成功！");
            });
          }
        });
      });
    }
    else
    {
      //alert(222);
      for (const i in this.searchForm.controls) {

        this.searchForm.controls[i].markAsDirty();
        this.searchForm.controls[i].updateValueAndValidity();
      }
    }
  }
}
