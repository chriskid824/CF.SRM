import { NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators,FormArray, FormControl} from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { SrmMaterialService } from '../../../business/srm/srm-material.service';
import { StorageService } from '../../../services/storage.service';
import { Role } from '../../system-manage/model/role';
import { RoleService } from 'src/app/business/system-manage/role.service';

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
      //roles: this._storageService.werks.split(','),
    });
  }

  initRoleList() {    
    this.werks = this._storageService.werks.split(',');
    console.log(this.werks );
  }


  add() {
    //$('#addBar').hide();
    var query = {
      material: this.searchForm.value["matnr"] == null ? "" : this.searchForm.value["matnr"],
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
    this._srmMaterialService.CheckMatnr(query).subscribe(result => {
      for (const i in this.searchForm.controls) {
        console.log(i);
        if(i!="note" && i!="sap_matnr")
        {
          this.searchForm.controls[i].markAsDirty();
          this.searchForm.controls[i].updateValueAndValidity();
        }
      }
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
      }
      console.log(material);
      if (this.searchForm.valid)
      {        
        this._srmMaterialService.AddMatnr(material).subscribe(result => {
          this._messageService.success("料號建立成功！");
        });
      }

    });
  }

}
