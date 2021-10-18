import { NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators,FormArray, FormControl} from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { SrmMaterialService } from '../../../business/srm/srm-material.service';
import { StorageService } from '../../../services/storage.service';
import { Role } from '../../system-manage/model/role';
import { RoleService } from 'src/app/business/system-manage/role.service';
import { NullLogger } from '@microsoft/signalr';
import { Result } from '@zxing/library';
import * as XLSX from 'xlsx';

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
  grouplist= [];
  unitlist= [];
  inputdata = [];
  arrayBuffer:any;
  file:File;

  constructor(
    private _formBuilder: FormBuilder,
    private _messageService: NzMessageService,
    private _srmMaterialService: SrmMaterialService,
    private _storageService: StorageService, 
    private _roleService: RoleService,) { 
           
    }
    

  ngOnInit(): void {  

    this.searchForm = this._formBuilder.group({
      sap_matnr:[null],
      matnr: [null],
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
      major_diameter: [null],
      minor_diameter: [null],
      //roles: this._storageService.werks.split(','),      
    });
    this.initRoleList();
  }

  initRoleList() {    
    this.werks = this._storageService.werks.split(',');
    //console.log(this._storageService);
    var query = {
      empid: this._storageService.userName == null ? "" : this._storageService.userName,
    }
    var querylist = {
      //material: {},
      page: 1,
      size: 999
    }
    this._srmMaterialService.GetGroupList(querylist).subscribe(result => {
      //console.log(result);
      this.grouplist = result["data"];
    });
    this._srmMaterialService.GetUnitList(querylist).subscribe(result => {
      //console.log(result);
      this.unitlist = result["data"];
    });

    this._srmMaterialService.GetEkgrp(query).subscribe(result => {
      this.searchForm.controls['ekgrp'].setValue(result['ekgry']);
    });
    

  }
  // daoru(evt: any) {
  //   const target: DataTransfer = <DataTransfer>(evt.target);
  //   if (target.files.length !== 1) {
  //     throw new Error('Cannot use multiple files');
  //   }
  //   const reader: FileReader = new FileReader();
  //   reader.readAsBinaryString(target.files[0]);
  //   reader.onload = (e: any) => {
  //     /* create workbook */
  //     const binarystr: string = e.target.result;
  //     const wb: XLSX.WorkBook = XLSX.read(binarystr, { type: 'binary' });

  //     /* selected the first sheet */
  //     const wsname: string = wb.SheetNames[0];
  //     const ws: XLSX.WorkSheet = wb.Sheets[wsname];
  //     console.log(wsname)
  //     console.log(ws)

  //     /* save data */
  //     const data = XLSX.utils.sheet_to_json(ws); // to get 2d array pass 2nd parameter as object {header: 1}
  //     console.log(data)
  //   };

    // const target: DataTransfer = <DataTransfer>(evt.target);
    // console.log(target.files.length);
    // if (target.files.length !== 1) throw new Error('Cannot use multiple files');
    // const reader: FileReader = new FileReader();
    // console.log(reader)
    // reader.onload = (e: any) => {
    //   /* read workbook */
    //   alert('456');
    //   const bstr: string = e.target.result;
    //   const wb: XLSX.WorkBook = XLSX.read(bstr, {type: 'binary'});

    //   /* grab first sheet */
    //   const wsname: string = wb.SheetNames[0];
    //   const ws: XLSX.WorkSheet = wb.Sheets[wsname];

    //   /* save data */
    //   this.inputdata = (XLSX.utils.sheet_to_json(ws, {header: 1}));
    //   console.log(this.inputdata)

    //   evt.target.value="" // 清空
    //};

  //}


  add() {
    if(this.searchForm.valid)
    {
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
        minor_diameter : this.searchForm.value['minor_diameter'],
        major_diameter : this.searchForm.value['major_diameter'],
      }
      this._srmMaterialService.Checkdata(material).subscribe(result => {
        console.log(material);
        if (this.searchForm.valid)
        {        
          this._srmMaterialService.AddMatnr(material).subscribe(result => {
            console.log(result);
            this._messageService.success("SRM料號："+result['srmMatnr1']+"，存檔成功！");
          });
        }
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
