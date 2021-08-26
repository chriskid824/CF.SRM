import { Component, OnInit, TemplateRef } from '@angular/core';
import { GridOptions } from 'ag-grid-community';
import { Role } from 'src/app/pages/system-manage/model/role';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { RoleService } from 'src/app/business/system-manage/role.service';
import { MenuService } from 'src/app/business/system-manage/menu.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Material } from 'src/app/pages/srm/model/material';
import { QotApiService } from './qot-api.service';
import { SrmQotService } from '../../../business/srm/srm-qot.service';
import { Process } from '../model/Process';
import { Surface } from '../model/Surface';
import { Other } from '../model/Other';
//import { NzTreeNodeOptions } from 'ng-zorro-antd/tree';
import { Qot, QotH, QotV, reject } from '../model/Qot';
import { ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NzTreeNodeOptions } from 'ng-zorro-antd/tree';
import { runInThisContext } from 'node:vm';
import { StorageService } from '../../../services/storage.service';
import { LayoutComponent } from '../../layout/layout/layout.component';


interface Food {
  value: string;
  viewValue: string;
}


@Component({
  selector: 'app-qot',
  templateUrl: './qot.component.html',
  styleUrls: ['./qot.component.less']
})
export class QotComponent implements OnInit {
  foods: Food[] = [
    { value: 'steak-0', viewValue: 'Steak' },
    { value: 'pizza-1', viewValue: 'Pizza' },
    { value: 'tacos-2', viewValue: 'Tacos' }
  ];
  @ViewChild('tree', { static: true })
  tree: any;
  matnrs = [];
  //departmentNode: NzTreeNodeOptions[] = [];
  selectedMatnr;
  matnrList: FormGroup = new FormGroup({});
  info2: FormGroup = new FormGroup({});
  info1: FormGroup = new FormGroup({});
  selectedDepartmentKey: string = '';
  editForm_Material: FormGroup = new FormGroup({});
  editForm_Process: FormGroup = new FormGroup({});
  editForm_Surface: FormGroup = new FormGroup({});
  editForm_Other: FormGroup = new FormGroup({});
  editForm_Reject: FormGroup = new FormGroup({});
  editedMatetial: Material = new Material();
  editedProcess: Process = new Process();
  editedSurface: Surface = new Surface();
  editedOther: Other = new Other();
  editedReject: reject = new reject();

  //reject: Other = new Material();

  tplModal: NzModalRef;
  rowData_Material;
  rowData_Process;
  rowData_Surface;
  rowData_Other;
  MT;//material
  PC;//process
  SF;//surface
  OT;//other
  MTApi;
  PCApi;
  SFApi;
  OTApi;
  gridApi;
  gridApi_Process;
  gridApi_Surface;
  gridApi_Other;
  columnDefs;
  defaultColDef;
  columnDefs_PROCESS;
  columnDefs_SURFACE;
  columnDefs_OTHER;
  rowSelection = "single";
  columnApi;
  columnApi_Process;
  columnApi_Surface;
  columnApi_Other;
  qotId;
  id;
  rfqid;
  vendorid;
  route;
  qot: QotH;
  qotv: Qot;
  matnrIndex;
  radioValue;
  matnrId;
  rowData_matnr = [];
  rowData_inforecord = [];
  Q;
  canModify;
  canModifymaterial;
  canModifyprocess;
  canModifysurface;
  canModifyother;
  IfCheck_M; //為顯示不回填
  IfCheck_P; //為顯示不回填
  IfCheck_S; //為顯示不回填
  IfCheck_O; //為顯示不回填
  nodes: NzTreeNodeOptions[] = [];
  public gridOptions: GridOptions;
  //editForm: FormGroup = new FormGroup({});
  constructor(private _formBuilder: FormBuilder,
    private _modalService: NzModalService,
    private _roleService: RoleService,
    private _menuService: MenuService,
    private _messageService: NzMessageService,
    private _srmQotService: SrmQotService,
    private activatedRoute: ActivatedRoute,
    private _router: Router,
    private _storageService: StorageService,
    private _layout: LayoutComponent,
  ) {
    this.Q = {
    }
    //this.activatedRoute.params.subscribe((params) => this.qotId = params['id']);
    //alert('qoooooooooooooooooooooooot = '+this.qotId)
    this.rowData_Material = [];
    this.data1 = [];
    this.rowData_Process = [];
    this.rowData_Surface = [];
    this.rowData_Other = [];
    /**********/
    /*material*/
  }



  //this.rowData_MATNR = [];
  ngOnInit(): void {
    //this.activatedRoute.params.subscribe((params) => this.qotId = params['id']);
    //alert(this.qotId)
    this.matnrIndex = 0;

    this.activatedRoute.queryParams
      .subscribe(params => {
        //console.log(params); // { orderby: "price" }
        this.id = params.id;
        this.vendorid = params.vendorid;
        this.rfqid = params.rfqid;
        //alert(this.vendorid)
        //alert(this.rfqid)
        //console.log(this.vendorid);
        //console.log(this.rfqid);
        //console.log(this.id); // price
        //alert('qqqqqqqqqqqqqqqqqqqotid='+this.id)
      }
      );
    this.matnrList = this._formBuilder.group({
      selectedMatnr: [null]
    });
    this.info1 = this._formBuilder.group({

    });
    this.info2 = this._formBuilder.group({});
    /* */
    var qot = {
      q: null
    }
    qot.q = this.Q;
    //qot.q = this.qot.qotId;
    qot.q.qotId = this.id;
    qot.q.rfqid = this.rfqid;
    qot.q.vendorid = this.vendorid;
    //this.matnrIndex = this._srmQotService.GetRowNum(qot) ;
    this._srmQotService.GetRowNum(qot).subscribe(result => {
      console.log(result);
      this.matnrIndex = result;
    });

    /* */
    this.init();
    this.initGrid();
    this.search();
    //this.gridOptions.columnDefs = this.columnDefsmaterial;
    // this.canModify = this.H.status == 1;


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

  data: QotH[] = [];
  editedRole: Role = new Role();
  data1: Material[] = [];

  page: number = 1;
  size: number = 10;
  total: number = 0;
  searchString = null;

  submitEdi() {

  }

  onGridReady(params) {
    function myRowClickedHandler(event) {
      //console.log('The row was clicked');
    }
    params.api.addEventListener('rowClicked', myRowClickedHandler);
    this.gridApi = params.api;
    this.columnApi = params.columnApi;
    //console.log(params.api);
    //this.gridApi.sizeColumnsToFit();
  }

  onGridReady_Surface(params) {
    function myRowClickedHandler(event) {
      //console.log('The row was clicked');
    }
    params.api.addEventListener('rowClicked', myRowClickedHandler);
    this.gridApi_Surface = params.api;
    this.columnApi_Surface = params.columnApi;

    //this.gridApi_Surface.sizeColumnsToFit();
  }

  onGridReady_Process(params) {
    function myRowClickedHandler(event) {
      //console.log('The row was clicked');
    }
    params.api.addEventListener('rowClicked', myRowClickedHandler);
    this.gridApi_Process = params.api;
    this.columnApi_Process = params.columnApi;
    //console.log(params.api);
    //this.gridApi_Process.sizeColumnsToFit();
  }
  onGridReady_Other(params) {
    function myRowClickedHandler(event) {
      //console.log('The row was clicked');
    }
    params.api.addEventListener('rowClicked', myRowClickedHandler);
    this.gridApi_Other = params.api;
    this.columnApi_Other = params.columnApi;
    //this.gridApi_Other.sizeColumnsToFit();
  }
  deleteMaterial() {
    let selectedNodes = this.gridApi.getSelectedNodes();
    let selectedData = selectedNodes.map(node => node.data);
    let selectedRows = this.gridApi.getSelectedRows();
    let temp = this.rowData_Material;
    for (var i = selectedRows.length - 1; i >= 0; i--) {
      for (var j = 0; j < this.rowData_Material.length; j++) {
        if (this.rowData_Material[j].material_name == selectedRows[i].material_name) {
          temp.splice(j, 1);
          break;
        }
      }
    }
    this.rowData_Material = temp;
    this.gridApi.setRowData(this.rowData_Material);
    //console.log(selectedRows);
    //this.gridApi.applyTransaction({ remove: selectedRows });
  }
  deleteProcess() {
    let selectedNodes = this.gridApi_Process.getSelectedNodes();
    let selectedData = selectedNodes.map(node => node.data);
    let selectedRows = this.gridApi_Process.getSelectedRows();
    let temp = this.rowData_Process;
    for (var i = selectedRows.length - 1; i >= 0; i--) {
      for (var j = 0; j < this.rowData_Process.length; j++) {
        if (this.rowData_Process[j].vendor == selectedRows[i].vendor) {
          temp.splice(j, 1);
          break;
        }
      }
    }
    this.rowData_Process = temp;
    this.gridApi_Process.setRowData(this.rowData_Process);
    //console.log(selectedRows);
    //this.gridApi.applyTransaction({ remove: selectedRows });
  }

  deleteSurface() {
    let selectedNodes = this.gridApi_Surface.getSelectedNodes();
    let selectedData = selectedNodes.map(node => node.data);
    let selectedRows = this.gridApi_Surface.getSelectedRows();
    let temp = this.rowData_Surface;
    for (var i = selectedRows.length - 1; i >= 0; i--) {
      for (var j = 0; j < this.rowData_Surface.length; j++) {
        if (this.rowData_Surface[j].vendor == selectedRows[i].vendor) {
          temp.splice(j, 1);
          break;
        }
      }
    }
    this.rowData_Surface = temp;
    this.gridApi_Surface.setRowData(this.rowData_Surface);
    //console.log(selectedRows);
    //this.gridApi.applyTransaction({ remove: selectedRows });
  }

  deleteOther() {
    let selectedNodes = this.gridApi_Other.getSelectedNodes();
    let selectedData = selectedNodes.map(node => node.data);
    let selectedRows = this.gridApi_Other.getSelectedRows();
    let temp = this.rowData_Other;
    for (var i = selectedRows.length - 1; i >= 0; i--) {
      for (var j = 0; j < this.rowData_Other.length; j++) {
        if (this.rowData_Other[j].vendor == selectedRows[i].vendor) {
          temp.splice(j, 1);
          break;
        }
      }
    }
    this.rowData_Other = temp;
    this.gridApi_Other.setRowData(this.rowData_Other);
    //console.log(selectedRows);
    //this.gridApi.applyTransaction({ remove: selectedRows });
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
  //addRole(title: TemplateRef<{}>, content: TemplateRef<{}>){}
  submitProcessList() {
    /**/
    for (const i in this.editForm_Process.controls) {
      this.editForm_Process.controls[i].markAsDirty();
      this.editForm_Process.controls[i].updateValueAndValidity();
    }
    //alert(this.editForm_Process.valid)
    if (this.editForm_Process.valid) {
      this.editedProcess.processno = this.editForm_Process.value['process_no'];
      this.editedProcess.price = this.editForm_Process.value['process_cost'];
      this.editedProcess.p_hour = this.editForm_Process.value['process_hour'];
      this.editedProcess.note = this.editForm_Process.value['process_remark'];
      this.editedProcess.machine = this.editForm_Process.value['process_machine'];
      console.info(this.editForm_Process.value);

      /*寫入grid */

      this.rowData_Process.push({
        "pProcessNum": this.editedProcess.processno,
        "pHours": this.editedProcess.p_hour,
        "pPrice": this.editedProcess.price,
        "pMachine": this.editedProcess.machine,
        "pNote": this.editedProcess.note
      });
      //alert('aaaaa+ '+ this.rowData_Process.value)
      //console.log(this.gridApi);
      //console.log('-------------');
      //console.log(this.gridApi_Process);
      this.gridApi_Process.setRowData(this.rowData_Process);
      this.tplModal.close();

    }
  }
  /**/
  submitMaterialList() {
    /**/
    for (const i in this.editForm_Material.controls) {
      this.editForm_Material.controls[i].markAsDirty();
      this.editForm_Material.controls[i].updateValueAndValidity();
    }
    //alert(this.editForm_Material.valid)
    if (this.editForm_Material.valid) {
      this.editedMatetial.name = this.editForm_Material.value['material_name'];
      this.editedMatetial.price = this.editForm_Material.value['material_price'];
      this.editedMatetial.cost = this.editForm_Material.value['material_cost'];
      this.editedMatetial.length = this.editForm_Material.value['material_length'];
      this.editedMatetial.width = this.editForm_Material.value['material_width'];
      this.editedMatetial.height = this.editForm_Material.value['material_height'];
      this.editedMatetial.density = this.editForm_Material.value['material_density'];
      this.editedMatetial.weight = this.editForm_Material.value['material_weight'];
      //this.editedMatetial.totalcost = this.editForm_Material.value['material_totalcost'];
      this.editedMatetial.note = this.editForm_Material.value['material_note'];
      console.info(this.editForm_Material.value);
      //alert('name = '+this.editedMatetial.name)
      /*寫入grid */

      this.rowData_Material.push({
        "mMaterial": this.editedMatetial.name,
        "mPrice": this.editedMatetial.price,
        "mCostPrice": this.editedMatetial.cost,
        "length": this.editedMatetial.length,
        "width": this.editedMatetial.width,
        "height": this.editedMatetial.height,
        "density": this.editedMatetial.density,
        //"M_TOTAL_COST":this.editedMatetial.totalcost,
        "weight": this.editedMatetial.weight,
        "note": this.editedMatetial.note,
      });
      //console.log('rowData_Material='+this.rowData_Material);

      this.gridApi.setRowData(this.rowData_Material);
      this.tplModal.close();
    }
  }
  /**/




  refresh() {
    this._roleService.getRoles(this.searchString, this.page, this.size)
      .subscribe((result: any) => { this.data = result['data']; this.total = result['count']; });
  }
  cancelEdit() {
    this.tplModal.close();
  }


  addMaterial(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    //this.editedRole = new Role();
    this.editForm_Material = this._formBuilder.group({
      //material_name: [this.editedMatetial.name, [Validators.required, Validators.maxLength(15)]],
      //0824 送出才檢核
      material_name: [this.editedMatetial.name,],
      material_price: [this.editedMatetial.price,],
      material_cost: [this.editedMatetial.cost,],
      material_length: [this.editedMatetial.length,],
      material_width: [this.editedMatetial.width,],
      material_height: [this.editedMatetial.height,],
      material_density: [this.editedMatetial.density,],
      material_weight: [this.editedMatetial.weight,],

      //material_totalcost: [this.editedMatetial.totalcost, [Validators.required]],
      material_note: [this.editedMatetial.note,]
      //menus: [[]]
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
    });
  }
  /*0730*/
  submitOtherList() {
    for (const i in this.editForm_Other.controls) {
      this.editForm_Other.controls[i].markAsDirty();
      this.editForm_Other.controls[i].updateValueAndValidity();
    }
    //alert(this.editForm_Other.valid)
    if (this.editForm_Other.valid) {
      this.editedOther.item = this.editForm_Other.value['other_item'];
      this.editedOther.price = this.editForm_Other.value['other_price'];
      this.editedOther.note = this.editForm_Other.value['other_note'];
      this.editedOther.description = this.editForm_Other.value['other_desc'];

      console.info(this.editForm_Other.value);
      //alert('name = '+this.editedOther.item)
      /*寫入grid */

      this.rowData_Other.push({
        "oItem": this.editedOther.item,
        "oPrice": this.editedOther.price,
        "oDescription": this.editedOther.description,
        "oNote": this.editedOther.note
      });

      this.gridApi_Other.setRowData(this.rowData_Other);
      this.tplModal.close();
    }
  }

  submitSurfaceList() {
    /**/
    for (const i in this.editForm_Surface.controls) {
      this.editForm_Surface.controls[i].markAsDirty();
      this.editForm_Surface.controls[i].updateValueAndValidity();
    }
    //alert(this.editForm_Surface.valid)
    if (this.editForm_Surface.valid) {
      this.editedSurface.process = this.editForm_Surface.value['surface_name'];
      this.editedSurface.price = this.editForm_Surface.value['surface_cost'];
      this.editedSurface.note = this.editForm_Surface.value['surface_note'];
      this.editedSurface.times = this.editForm_Surface.value['surface_times'];


      //console.info(this.editForm_Surface.value);
      //alert('name = '+this.editedSurface.process)
      /*寫入grid */

      this.rowData_Surface.push({
        "sProcess": this.editedSurface.process,
        "sTimes": this.editedSurface.times,
        "sPrice": this.editedSurface.price,
        "sNote": this.editedSurface.note
      });

      this.gridApi_Surface.setRowData(this.rowData_Surface);
      this.tplModal.close();
    }
  }
  checktype(number: number) {
    if (isNaN(number)) {
      alert("欄位格式錯誤");
      return;
    }
  }
  addSurface(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.editedSurface = new Surface();
    this.editForm_Surface = this._formBuilder.group({
      //material_name: [this.editedMatetial.name, [Validators.required, Validators.maxLength(15)]],
      surface_name: [this.editedSurface.process,],
      surface_times: [this.editedSurface.times,],
      surface_cost: [this.editedSurface.price,],
      surface_note: [this.editedSurface.note]
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
    });
  }

  addOther(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.editedOther = new Other();
    this.editForm_Other = this._formBuilder.group({
      //material_name: [this.editedMatetial.name, [Validators.required, Validators.maxLength(15)]],
      other_item: [this.editedOther.item,],
      other_desc: [this.editedOther.description],
      other_price: [this.editedOther.price,],
      other_note: [this.editedOther.note,]
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
    });
  }
  /*0730*/
  addProcess(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.editForm_Process = this._formBuilder.group({
      //material_name: [this.editedMatetial.name, [Validators.required, Validators.maxLength(15)]],
      process_no: [this.editedProcess.processno,],
      process_cost: [this.editedProcess.price,],
      process_hour: [this.editedProcess.p_hour,],
      process_machine: [this.editedProcess.machine,],
      process_remark: [this.editedProcess.note],
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
    });
  }
  /**/
  getqot() {
    var qot = {
      q: null, material: null, process: null, surface: null, other: null
    }

    qot.q = this.Q;
    //this.Q = result["h"];
    //qot["q"] = this.Q;
    qot.q.vendorId = this.vendorid;  //0825 add
    qot.q.matnrId = this.radioValue; //0825 add
    qot.q.LASTUPDATEBY = this._storageService.userName;
    qot.q.QotId = this.id;
    qot.q.RfqId = this.rfqid;
    /*不回填flag*/
    qot.q.MEmptyFlag = ($('#chkreject_material-input').prop("checked")) ? "X" : "";
    qot.q.PEmptyFlag = ($('#chkreject_process-input').prop("checked")) ? "X" : "";
    qot.q.SEmptyFlag = ($('#chkreject_surface-input').prop("checked")) ? "X" : "";
    qot.q.OEmptyFlag = ($('#chkreject_other-input').prop("checked")) ? "X" : "";
    /*
    var rfq = {
      h: null, m: null, v: null
    }
    var date = new Date();
    this.H.SOURCER = this.formDetail.value["sourcer"];

    rfq.h = this.H;
    console.log('dead' + this.formDetail.get('deadline').value);
    if (this.formDetail.value["deadline"] != null) {
      var deadline = new Date(this.formDetail.value["deadline"]);
      rfq.h.deadLine = deadline.getFullYear() + '-' + (deadline.getMonth() + 1) + '-' + deadline.getDate();
    } else {
      rfq.h.deadLine = null;
    }
    rfq.h.LASTUPDATEBY = this._storageService.userName;
    */

    qot.material = []
    qot.process = [];
    qot.surface = [];
    qot.other = [];

    this.gridApi.forEachNode(node => qot.material.push(node.data));
    this.gridApi_Process.forEachNode(node => qot.process.push(node.data));
    this.gridApi_Surface.forEachNode(node => qot.surface.push(node.data));
    this.gridApi_Other.forEachNode(node => qot.other.push(node.data));
    return qot;
  }

  Save() {
    /*檢核griddata若grid沒有資料須勾選不回填*/

    /**/
    var qot = this.getqot();
    this._srmQotService.Save(qot).subscribe(result => {
      //console.log(result);
      alert('保存成功');
      this._layout.navigateTo('qotlist');
      this._router.navigate(['srm/qotlist']);
    });
  }
  /*RejectQot(){  
    
  
    var qot = {
      q:null
    }
    qot.q = this.Q;
    //qot.q = this.qot.qotId;
    qot.q.LASTUPDATEBY = this._storageService.userName;
    qot.q.QotId = this.id;
    qot.q.RfqId = this.rfqid;
    this._srmQotService.Reject(qot).subscribe(result => {
      console.log(result);
      alert('拒絕報價成功');
      this._layout.navigateTo('qotlist');
      this._router.navigate(['srm/qotlist']);
    });
  }*/
  Reject(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.editForm_Reject = this._formBuilder.group({
      //material_name: [this.editedMatetial.name, [Validators.required, Validators.maxLength(15)]],
      reject_reason: [this.editedReject.reason, [Validators.required]]
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
    });
  }
  submitReject() {
    for (const i in this.editForm_Reject.controls) {
      this.editForm_Reject.controls[i].markAsDirty();
      this.editForm_Reject.controls[i].updateValueAndValidity();
    }

    //alert(this.editForm_Reject.value["reject_reason"]);
    if (!this.editForm_Reject.value["reject_reason"]) {
      alert("拒絕原因必填");
      return;
    }
    /*var qot = {
      q:null
    }
    qot.q = this.Q;
    //qot.q = this.qot.qotId;
    qot.q.LASTUPDATEBY = this._storageService.userName;
    qot.q.QotId = this.id;
    qot.q.RfqId = this.rfqid;
    //qot.q.vendorId = this.vendorid;
    //qot.q.mat:*/
    var qot = this.getqot();
    qot.q.reason = this.editForm_Reject.value["reject_reason"];
    this._srmQotService.Reject(qot).subscribe(result => {
      //console.log(result);
      alert('拒絕報價成功');
      this._layout.navigateTo('qotlist');
      this._router.navigate(['srm/qotlist']);

    });
    this.tplModal.close();
  }

  /*saveqotmatnr() {
    for (const i in this.editForm_Material.controls) {
      this.editForm_Material.controls[i].markAsDirty();
      this.editForm_Material.controls[i].updateValueAndValidity();
    }
    //alert(this.editForm.valid)
    if (this.editForm_Material.valid) {
      this.editedMatetial.name = this.editForm_Material.value['material_name'];
      //alert('aa = '+this.editedMatetial.name)
      this.editedMatetial.price = this.editForm_Material.value['material_price'];
      this.editedMatetial.cost = this.editForm_Material.value['material_cost'];
      this.editedMatetial.length = this.editForm_Material.value['material_length'];
      this.editedMatetial.width = this.editForm_Material.value['material_width'];
      this.editedMatetial.height = this.editForm_Material.value['material_height'];
      this.editedMatetial.density = this.editForm_Material.value['material_density'];
      this.editedMatetial.weight = this.editForm_Material.value['material_weight'];
      //this.editedMatetial.totalcost = this.editForm.value['material_totalcost'];
      this.editedMatetial.note = this.editForm_Material.value['material_note'];
    }
    console.log("saveqotmatnr");
    //alert("saveqotmatnr");
    var qot = {
      material: null,process :null,surface:null,other:null
    }
    //var date = new Date();
    //qot.material = this.MT;
    qot.material = {
      MPrice: this.editForm_Material.value['material_price'],
      MCostPrice: this.editForm_Material.value['material_cost'],
      Length: this.editForm_Material.value['material_length'],
      Width: this.editForm_Material.value['material_width'],
      Height: this.editForm_Material.value['material_height'],
      Density: this.editForm_Material.value['material_density'],
      Weight: this.editForm_Material.value['material_weight'],
      Note: this.editForm_Material.value['material_note'],
    };
    //alert(qot.material.MMaterial);
    console.log(this.MTApi);
    this.MTApi.forEachNode(node => console.log(node));
    this.MTApi.forEachNode(node => qot.material.push(node.data));
    //alert('aaaa=' + qot.material)
    //console.log('aaaa='+ qot.material)
    
    this._srmQotService.Save(qot).subscribe(result => {
      console.log(result);
    });

  }*/



  //0812
  refreshtree() {
    this.tree.initNodes();
  }

  selectedChanged(array: QotH[]) {
    this.data = array;
  }


  init() {

    //alert(this.radioValue)
    //alert('aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa='+ 'init')
    this._srmQotService.GetQotData(this.id, this.rfqid, this.vendorid).subscribe(result => {
      //console.log(result);
      this.qotv = result;
      //console.info("resultttttttt="+this.qotv);
      //console.log(this.qotv.m);
      //console.log(this.qotv);
      //console.log(this.qotv.q);
      //console.log(this.qotv.q[0]);
      //console.log('test');
      //console.log(this.qotv.q[0].createBy);
      this.matnrs = [];
      this.qotv.m.forEach(row => this.matnrs.push({ label: row.matnr, value: row.matnrId }));//???
      //console.log(this.matnrs);
      this.matnrList.setValue({ selectedMatnr: this.matnrs[0].value });//???
      //console.log(this.matnrs[0].value);
      this.nodes = [];
      //this.nodes = [{ title: this.qotv.q[0].rfqNum, key: null, icon: 'global', expanded: true, children: [] }];;
      this.nodes = [{ title: this.qotv.q[this.matnrIndex].rfqNum, key: null, icon: 'global', expanded: true, children: [] }];;

      this.qotv.m.forEach((row, index) => this.nodes[0].children.push({ title: row.matnr, key: row.matnrId.toString(), icon: 'appstore', children: [], index: index }))

      //alert('status')
      //alert(this.qotv.q[this.matnrIndex].status)

      this.IfCheck_M = false;
      this.IfCheck_P = false;
      this.IfCheck_S = false;
      this.IfCheck_O = false;
      //material
      console.log('----*****----')
      console.log(this.qotv.q[this.matnrIndex])

      if (this.qotv.q[this.matnrIndex].status != "初始") {
        this.canModify = false;
        this.canModifymaterial = false;
        this.canModifyprocess = false;
        this.canModifysurface = false;
        this.canModifyother = false;

        if (this.qotv.q[this.matnrIndex].mEmptyFlag == "X") {
          this.IfCheck_M = true;
        }
        else {
          this.IfCheck_M = false;
         
        }
        if (this.qotv.q[this.matnrIndex].pEmptyFlag == "X") {
          this.IfCheck_P = true;
        }
        else {
          this.IfCheck_P = false;
        }
        if (this.qotv.q[this.matnrIndex].sEmptyFlag == "X") {
          this.IfCheck_S = true;
        }
        else {
          this.IfCheck_S = false;
        }
        if (this.qotv.q[this.matnrIndex].oEmptyFlag == "X") {
          this.IfCheck_O = true;
        }
        else {
          this.IfCheck_O = false;
        }

      }
      else {
        this.canModify = true;
        this.canModifymaterial = true;
        this.canModifyprocess = true;
        this.canModifysurface = true;
        this.canModifyother = true;

        if (this.qotv.q[this.matnrIndex].mEmptyFlag == "X") {
          this.IfCheck_M = true;
          this.canModifymaterial = false;

        }
        else {
          this.IfCheck_M = false;
          this.canModifymaterial = true;
        }
        if (this.qotv.q[this.matnrIndex].pEmptyFlag == "X") {
          this.IfCheck_P = true;
          this.canModifyprocess = false;
        }
        else {
          this.IfCheck_P = false;
          this.canModifyprocess = true;
        }
        if (this.qotv.q[this.matnrIndex].sEmptyFlag == "X") {
          this.IfCheck_S = true;
          this.canModifysurface = false;
        }
        else {
          this.IfCheck_S = false;
          this.canModifysurface = true;
        }
        if (this.qotv.q[this.matnrIndex].oEmptyFlag == "X") {
          this.IfCheck_O = true;
          this.canModifyother = false;
        }
        else {
          this.IfCheck_O = false;
          this.canModifyother = true;
        }
      }
      /*alert( this.IfCheck_M)
      alert( this.IfCheck_P)
      alert( this.IfCheck_S)
      alert( this.IfCheck_O)*/
      // { title: department.name, key: department.id, icon: 'appstore', children: [] };
      //this.radioValue = this.matnrs[0].value;
    });
    //console.log('----------------------------init-----------------------------')
    console.info(this.qot);

    var query = {
      qotid: this.id,
      matnrId: this.radioValue,
      vendorid: this.vendorid,
      rfqid: this.rfqid
    };
    this._srmQotService.GetQotDetail(query).subscribe(result => {
      this.rowData_Material = result["material"];
      this.rowData_Process = result["process"];
      this.rowData_Other = result["other"];
      this.rowData_Surface = result["surface"];
    });
    //console.log("init!!!!!!!!!!!!!!!")
    //console.log(this.rowData_Material)
  }
  initGrid() {
    this.columnDefs = [
      {
        headerName: "素材材質",
        field: "mMaterial",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
        editable: this.canModify,
      },
      {
        headerName: "材料單價",
        field: "mPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "材料成本價",
        field: "mCostPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "長",
        field: "length",
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "寬",
        field: "width",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "高",
        field: "height",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "密度",
        field: "density",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "重量",
        field: "weight",
        enableRowGroup: true,
        cellClass: "show-cell",
        //editable: true,
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "備註",
        field: "note",
        enableRowGroup: true,
        cellClass: "show-cell",
        //editable: true,
        width: "150px",
        editable: this.canModify,
      }

    ]
    this.defaultColDef = {
      filter: "agTextColumnFilter",
      allowedAggFuncs: ['sum', 'min', 'max'],
      enableValue: true,
      enableRowGroup: true,
      enablePivot: true,
      // floatingFilter: true,
      resizable: true,
      rowSelection: "multiple",
      wrapText: true,
    };
    /*process*/
    this.columnDefs_PROCESS = [
      {
        headerName: "工序代碼",
        field: "pProcessNum",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "250px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
        editable: this.canModify,
      },
      {
        headerName: "工時(時)",
        field: "pHours",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "單價(時)",
        field: "pPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "機台",
        field: "pMachine",
        enableRowGroup: true,
        cellClass: "show-cell",
        autoHeight: true,
        wrapText: true,
        editable: this.canModify,
      },
      {
        headerName: "備註",
        field: "pNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      }
    ]
    /*surface*/
    this.columnDefs_SURFACE = [
      {
        headerName: "工序",
        field: "sProcess",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "250px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
        editable: this.canModify,
      },
      {
        headerName: "次數",
        field: "sTimes",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "單價(時)",
        field: "sPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "備註",
        field: "sNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      }
    ]
    /*other*/
    this.columnDefs_OTHER = [
      {
        headerName: "項目",
        field: "oItem",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "250px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
        editable: this.canModify,
      },
      {
        headerName: "說明",
        field: "oDescription",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "單價",
        field: "oPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "備註",
        field: "oNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      }
    ]
    /**********/
  }
  changedMatnr(value) {
    //alert('changedMatnr')
    //console.log('-----------changedMatnr-------------');
    //console.log(value);
    this.matnrList.setValue({ selectedMatnr: value.keys[0] });
    //if (value.node.origin.index != null) {
    this.matnrIndex = value.node.origin.index;
    //}
    console.log('!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!')
    //console.log(this.matnrIndex);
    //alert(this.matnrIndex)
    console.log(this.qotv.q)
    //alert( this.qotv.q[this.matnrIndex].qotNum)
    this.search();
  }

  search() {
    //alert('-------search-------')
    this.radioValue = this.matnrList.get('selectedMatnr').value;
    //alert(this.radioValue)
    this.rowData_matnr = [];
    this.rowData_Material = [];
    this.rowData_Process = [];
    this.rowData_Surface = [];
    this.rowData_Other = [];
    //alert('ppppppppp')
    //alert(this.radioValue)
    //alert(this.id)
    //alert(this.radioValue)
    if (!(this.id) || !(this.radioValue)) {
      return;
    }
    //alert('-------query-------')
    var query = {
      qotid: this.id,
      matnrId: this.radioValue,
      vendorid: this.vendorid,
      rfqid: this.rfqid
    };
    //alert('---------matnrId----------------');
    //alert(this.radioValue);
    //alert(query)
    this.IfCheck_M = false;
    this.IfCheck_P = false;
    this.IfCheck_S = false;
    this.IfCheck_O = false;
    this._srmQotService.GetQotDetail(query).subscribe(result => {
      //alert('GetQotDetail')
      this.rowData_matnr = [result["matnr"]];
      this.rowData_Material = result["material"];
      this.rowData_Process = result["process"];
      this.rowData_Surface = result["surface"];
      this.rowData_Other = result["other"];
      console.log('---------search---------');
      console.log(result);
      console.log([result["qot"]]);
      console.log([result["qot"]][0].status);

      //alert([result["qot"]][0].mEmptyFlag);
      //alert([result["qot"]][0].SEmptyFlag);

      if ([result["qot"]][0].status == "1") {
        this.canModify = true;
        this.canModifymaterial = true;
        this.canModifyprocess = true;
        this.canModifysurface = true;
        this.canModifyother = true;

        
        if ([result["qot"]][0].mEmptyFlag == "X") {
          this.IfCheck_M = true;
          this.canModifymaterial = false;
        }
        else {
          this.IfCheck_M = false;
          this.canModifymaterial = true;
        }

        if ([result["qot"]][0].pEmptyFlag == "X") {
          this.IfCheck_P = true;
          this.canModifyprocess = false;
        }
        else {
          this.IfCheck_P = false;
          this.canModifyprocess = true;
        }

        if ([result["qot"]][0].sEmptyFlag == "X") {
          this.IfCheck_S = true;
          this.canModifysurface = false;
        }
        else {
          this.IfCheck_S = false;
          this.canModifysurface = true;
        }
        if ([result["qot"]][0].oEmptyFlag == "X") {
          this.IfCheck_O = true;
          this.canModifyother = false;
        }
        else {
          this.IfCheck_O = false;
          this.canModifyother = true;
        }
      }
      else {
        this.canModify = false;
        this.canModifymaterial = false;
        this.canModifyprocess = false;
        this.canModifysurface = false;
        this.canModifyother = false;

        if ([result["qot"]][0].mEmptyFlag == "X") {
          this.IfCheck_M = true;
        }
        else {
          this.IfCheck_M = false;
        }

        if ([result["qot"]][0].pEmptyFlag == "X") {
          this.IfCheck_P = true;
        }
        else {
          this.IfCheck_P = false;
        }

        if ([result["qot"]][0].sEmptyFlag == "X") {
          this.IfCheck_S = true;
        }
        else {
          this.IfCheck_S = false;
        }
        if ([result["qot"]][0].oEmptyFlag == "X") {
          this.IfCheck_O = true;
        }
        else {
          this.IfCheck_O = false;
        }

      }

    });
    //this.init();
    //console.log(result);
    /*alert( 'search')
    alert( this.IfCheck_M)
    alert( this.IfCheck_P)
    alert( this.IfCheck_S)
    alert( this.IfCheck_O)*/
  }

  BackQot() {
    window.open('../srm/qotlist');
  }
  SendQot() {

    //1.需先檢核內容
    //2.檢查通過後執行SAVE及變更狀態
    var qot = this.getqot();

    if (((qot.material.length) == 0) && (!$('#chkreject_material-input').prop("checked"))) {
      alert('材料至少需一筆或請勾選材料不回填!'); return;
    }
    if (((qot.process.length) == 0) && (!$('#chkreject_process-input').prop("checked"))) {
      alert('加工至少需一筆或請勾選加工不回填!'); return;
    }
    if (((qot.surface.length) == 0) && (!$('#chkreject_surface-input').prop("checked"))) {
      alert('表面處理至少需一筆或請勾選表面處理不回填!'); return;
    }
    if (((qot.other.length) == 0) && (!$('#chkreject_other-input').prop("checked"))) {
      alert('其他費用至少需一筆或請勾選其他費用不回填!'); return;
    }

    //material
    for (var i = 0; i < qot.material.length; i++) {
      if (!qot.material[i].mMaterial) {
        alert("素材材質未填");
        //alert("料號" + rfq.m[i].srmMatnr1 + "數量未填");
        return;
      }
      if (!qot.material[i].mCostPrice) {
        alert("素材材質" + qot.material[i].mMaterial + "材料成本價未填");
        return;
      }
      if (isNaN(qot.material[i].mCostPrice)) {
        alert("素材材質" + qot.material[i].mMaterial + "材料成本價格式錯誤");
        return;
      }
    }

    //process
    for (var i = 0; i < qot.process.length; i++) {
      if (!qot.process[i].pProcessNum) {
        alert("工序代碼未填");
        return;
      }
      if (!qot.process[i].pHours) {
        alert("工序代碼" + qot.process[i].pProcessNum + "工時(時)未填");
        return;
      }
      if (isNaN(qot.process[i].pHours)) {
        alert("工序代碼" + qot.process[i].pProcessNum + "工時(時)格式錯誤");
        return;
      }
      if (!qot.process[i].pPrice) {
        alert("工序代碼" + qot.process[i].pProcessNum + "單價(時)未填");
        return;
      }
      if (isNaN(qot.process[i].pPrice)) {
        alert("工序代碼" + qot.process[i].pProcessNum + "單價(時)格式錯誤");
        return;
      }
    }

    //surface
    for (var i = 0; i < qot.surface.length; i++) {
      if (!qot.surface[i].sProcess) {
        alert("工序未填");
        return;
      }
      if (!qot.surface[i].sTimes) {
        alert("工序" + qot.surface[i].sProcess + "次數未填");
        return;
      }
      if (isNaN(qot.surface[i].sTimes)) {
        alert("工序" + qot.surface[i].sProcess + "次數格式錯誤");
        return;
      }
      if (!qot.surface[i].sPrice) {
        alert("工序" + qot.surface[i].sProcess + "單價(時)未填");
        return;
      }
      if (isNaN(qot.surface[i].sPrice)) {
        alert("工序" + qot.surface[i].sProcess + "單價(時)格式錯誤");
        return;
      }
    }

    //other
    for (var i = 0; i < qot.other.length; i++) {
      if (!qot.other[i].oItem) {
        alert("項目未填");
        return;
      }
      if (!qot.other[i].oPrice) {
        alert("項目" + qot.other[i].oItem + "單價未填");
        return;
      }
      if (isNaN(qot.other[i].oPrice)) {
        alert("項目" + qot.other[i].oItem + "單價格式錯誤");
        return;
      }
    }
    //????
    this._srmQotService.Send(qot).subscribe(result => {
      alert('送出成功');
      //window.close();
      this._layout.navigateTo('qot');
      this._router.navigate(['srm/qotlist']);
    });
  }
  checkCheckBoxvalue_M(event) {

    console.log('---checkCheckBoxvalue---')
    console.log(event)
    console.log(event.checked)
    //alert($('#chkreject_material-input').prop("checked"))
    if (event.checked) {
      this.canModifymaterial = false;
    }
    else {
      this.canModifymaterial = true;
    }
  }

  checkCheckBoxvalue_P(event) {
    if (event.checked) {
      this.canModifyprocess = false;
    }
    else {
      this.canModifyprocess = true;
    }
  }
  checkCheckBoxvalue_S(event) {
    if (event.checked) {
      this.canModifysurface = false;
    }
    else {
      this.canModifysurface = true;
    }
  }
  checkCheckBoxvalue_O(event) {
    if (event.checked) {
      this.canModifyother = false;
    }
    else {
      this.canModifyother = true;
    }
  }
}
