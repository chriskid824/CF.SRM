import { Component, OnInit, TemplateRef } from '@angular/core';
import { GridOptions } from 'ag-grid-community';
import { Role } from 'src/app/pages/system-manage/model/role';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { RoleService } from 'src/app/business/system-manage/role.service';
import { MenuService } from 'src/app/business/system-manage/menu.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Material } from 'src/app/pages/srm/model/material';
import { QotApiService} from './qot-api.service';
import { SrmQotService } from '../../../business/srm/srm-qot.service';


@Component({
  selector: 'app-qot', 
  templateUrl: './qot.component.html',
  styleUrls: ['./qot.component.less']
})
export class QotComponent implements OnInit {
  editForm: FormGroup = new FormGroup({});
  tplModal: NzModalRef;
  rowData_MATNR;
  MT;//material
  PC;//process
  SF;//surface
  OT;//other
  MTApi;
  PCApi;
  SFApi;
  OTApi;
  gridApi;
  gridApi_VENDOR;
  public gridOptions: GridOptions;
  //editForm: FormGroup = new FormGroup({});
  constructor(private _formBuilder: FormBuilder,
    private _modalService: NzModalService,
    private _roleService: RoleService,
    private _menuService: MenuService,
    private _messageService: NzMessageService,
    private _srmQotService: SrmQotService,
 

  ) { this.rowData_MATNR = []; }
     //this.rowData_MATNR = [];
  ngOnInit(): void {
    //this.gridOptions.columnDefs = this.columnDefsmaterial;
  }

  columnDefsmaterial = [
    { field: '序號', resizable: true },
    { field: '素材材質', resizable: true },
    { field: '材料單價', resizable: true },
    { field: '材料成本價', resizable: true },
    { field: '長', resizable: true },
    { field: '寬', resizable: true },
    { field: '高', resizable: true },
    { field: '密度', resizable: true },
    { field: '重量', resizable: true },
    { field: '總金額', resizable: true },
    { field: '備註', resizable: true }
  ];

  rowDatamaterial = [
    { 序號: '01', 素材材質: '鐵', 材料單價: '200', 材料成本價: '180', 長: '2', 寬: '3', 高: '5', 密度: '1.5', 重量: '50', 總金額: '100', 備註: 'AA' },
  ];

  columnDefsprocess = [
    { field: '序號', resizable: true },
    { field: '工序代碼', resizable: true },
    { field: '工時(時)', resizable: true },
    { field: '單價(時)', resizable: true },
    { field: '機台', resizable: true },
    { field: '備註', resizable: true }
  ];
  MatnrList;

  OriMatnrList;

  rowDataprocess = [
    { 序號: '01', 工序代碼: '0101', '工時(時)': '5', '單價(時)': '20', 機台: 'machine', 備註: 'A' },
  ];
  columnDefssurface = [
    { field: '序號', resizable: true },
    { field: '工序', resizable: true },
    { field: '次數', resizable: true },
    { field: '單價(時)', resizable: true },
    { field: '備註', resizable: true }
  ];

  rowDatasurface = [
    { 序號: '1', 工序: '工序A', 次數: '1', '單價(時)': '20', 備註: 'A' },
  ];

  columnDefsother = [
    { field: '序號', resizable: true },
    { field: '項目', resizable: true },
    { field: '說明', resizable: true },
    { field: '單價', resizable: true },
    { field: '備註', resizable: true }
  ];

  rowDataother = [
    { 序號: '1', 項目: '項目1', 說明: 'NA', 單價: '20', 備註: 'A' },
  ];
  OpenMaterial() {

  }
  //tplModal: NzModalRef;


  data: Role[] = [];
  editedRole: Role = new Role();
  data1: Material[] = [];
  editedMatetial: Material = new Material();
  page: number = 1;
  size: number = 10;
  total: number = 0;
  searchString = null;

  submitEdi() {

  }
  /**/
  add(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
      nzClosable: true,
      nzMaskClosable: false
    });
  }
  /**/
  submitMatnrList() {
    /**/
    for (const i in this.editForm.controls) {
      this.editForm.controls[i].markAsDirty();
      this.editForm.controls[i].updateValueAndValidity();
    }
    alert(this.editForm.valid)
    if (this.editForm.valid) {
      this.editedMatetial.name = this.editForm.value['material_name'];
      this.editedMatetial.price = this.editForm.value['material_price'];
      console.info(this.editForm);
      alert('name = '+this.editedMatetial.name)
      //this.editedMatetial.cost = this.editForm.value['remark'];
      //this.editedMatetial.length = this.editForm.value['remark'];
      //this.editedMatetial.width = this.editForm.value['remark'];
      //this.editedMatetial.height = this.editForm.value['remark'];
      //this.editedMatetial.density = this.editForm.value['remark'];
      //this.editedMatetial.weight = this.editForm.value['remark'];
      //this.editedMatetial.totalcost = this.editForm.value['remark'];
      //this.editedMatetial.note = this.editForm.value['remark'];
      
      if (this.editedMatetial.id) {
      //  this._roleService.updateRole(this.editedRole)
      //    .subscribe(result => {
      //      this.refresh(); this.tplModal.close();
      //      this._messageService.success("更新成功！");
      //    });
      //} else {
      //  this._roleService.addRole(this.editedRole)
      //    .subscribe(result => {
      //      this.refresh(); this.tplModal.close();
      //      this._messageService.success("添加成功！");
      //    });
      }
    }
    /**/
    alert('aaaa')
    alert('aaaacc=' + this.rowData_MATNR)
    console.log(this.rowData_MATNR);
    for (var i = 0; i < this.MatnrList.length; i++) {
      if (this.MatnrList[i].checked == true) {
        console.log(this.MatnrList[i].label);
        if (this.rowData_MATNR.filter(item => item.matnr.toUpperCase().indexOf(this.MatnrList[i].label.toUpperCase()) >= 0).length == 0) {
          this.rowData_MATNR.push(this.OriMatnrList.filter(item => item.matnr == this.MatnrList[i].label)[0]);
        }
      }
    }
    this.rowData_MATNR.forEach(row => row.volume = row.length + 'X' + row.width + 'X' + row.height);
    this.gridApi.setRowData(this.rowData_MATNR);
    console.log(this.rowData_MATNR);
    this.tplModal.close();
  }
  /**/

  submitEdit() {
    alert('submitEdit')
    for (const i in this.editForm.controls) {
      this.editForm.controls[i].markAsDirty();
      this.editForm.controls[i].updateValueAndValidity();
    }
    alert(this.editForm.valid)
    //if (this.editForm.valid) {
    //  this.editedMatetial.name = this.editForm.value['material_name'];
    //  this.editedMatetial.price = this.editForm.value['material_price'];
    //  this.editedMatetial.cost = this.editForm.value['material_cost'];
    //  this.editedMatetial.length = this.editForm.value['material_length'];
    //  this.editedMatetial.width = this.editForm.value['material_width'];
    //  this.editedMatetial.height = this.editForm.value['material_height'];
    //  this.editedMatetial.density = this.editForm.value['material_density'];
    //  this.editedMatetial.weight = this.editForm.value['material_weight'];
    //  this.editedMatetial.totalcost = this.editForm.value['material_totalcost'];
    //  this.editedMatetial.note = this.editForm.value['material_note'];


    //  //this.editedRole.menus = this.editForm.value['menus'].join(',');
    //  //?? insert db
    //  //if (this.editedMatetial.id) {
    //  //  this._roleService.updateRole(this.editedMatetial)
    //  //    .subscribe(result => {
    //  //      this.refresh(); this.tplModal.close();
    //  //      this._messageService.success("更新成功！");
    //  //    });
    //  //} else {
    //  //  this._roleService.addRole(this.editedRole)
    //  //    .subscribe(result => {
    //  //      this.refresh(); this.tplModal.close();
    //  //      this._messageService.success("添加成功！");
    //  //    });
    //  //}
    //}
  }



  refresh() {
    this._roleService.getRoles(this.searchString, this.page, this.size)
      .subscribe((result: any) => { this.data = result['data']; this.total = result['count']; });
  }
  cancelEdit() {
    this.tplModal.close();
  }

  /**
   * /
   * @param title
   * @param content
   */
  addRole1(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.editedMatetial = new Material();
    this.editForm = this._formBuilder.group({
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
    });
  }

  addRole(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    //this.editedRole = new Role();
    //this.editForm = this._formBuilder.group({
    //  roleName: [this.editedRole.name, [Validators.required, Validators.maxLength(15)]],
    //  remark: [this.editedRole.remark, [Validators.maxLength(30)]],
    //  menus: [[]]
    //});
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
    });
  }
  /**/
  saveqotmatnr() {
    for (const i in this.editForm.controls) {
      this.editForm.controls[i].markAsDirty();
      this.editForm.controls[i].updateValueAndValidity();
    }
    //alert(this.editForm.valid)
    if (this.editForm.valid) {
      this.editedMatetial.name = this.editForm.value['material_name'];
      alert('aa = '+this.editedMatetial.name)
      this.editedMatetial.price = this.editForm.value['material_price'];
      this.editedMatetial.cost = this.editForm.value['material_cost'];
      this.editedMatetial.length = this.editForm.value['material_length'];
      this.editedMatetial.width = this.editForm.value['material_width'];
      this.editedMatetial.height = this.editForm.value['material_height'];
      this.editedMatetial.density = this.editForm.value['material_density'];
      this.editedMatetial.weight = this.editForm.value['material_weight'];
      this.editedMatetial.totalcost = this.editForm.value['material_totalcost'];
      this.editedMatetial.note = this.editForm.value['material_note'];
    }
    console.log("saveqotmatnr");
    alert("saveqotmatnr");
    var qot = {
      material: null,process :null,surface:null,other:null
    }
    //var date = new Date();
    //qot.material = this.MT;
    qot.material = {
      MPrice: this.editForm.value['material_price'],
      MCostPrice: this.editForm.value['material_cost'],
      Length: this.editForm.value['material_length'],
      Width: this.editForm.value['material_width'],
      Height: this.editForm.value['material_height'],
      Density: this.editForm.value['material_density'],
      Weight: this.editForm.value['material_weight'],
      Note: this.editForm.value['material_note'],
    };
    alert(qot.material.MMaterial);
    console.log(this.MTApi);
    this.MTApi.forEachNode(node => console.log(node));
    this.MTApi.forEachNode(node => qot.material.push(node.data));
    alert('aaaa=' + qot.material)
    //console.log('aaaa='+ qot.material)
    
    this._srmQotService.SaveQotMatnr(qot).subscribe(result => {
      console.log(result);
    });

  }

  /**/
}
