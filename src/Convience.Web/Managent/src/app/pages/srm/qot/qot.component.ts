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
import { Process } from '../model/Process';
import { Surface } from '../model/Surface';
import { Other } from '../model/Other';


@Component({
  selector: 'app-qot', 
  templateUrl: './qot.component.html',
  styleUrls: ['./qot.component.less']
})
export class QotComponent implements OnInit {
  editForm_Material: FormGroup = new FormGroup({});
  editForm_Process: FormGroup = new FormGroup({});
  editForm_Surface: FormGroup = new FormGroup({});
  editForm_Other: FormGroup = new FormGroup({});
  editedMatetial: Material = new Material();
  editedProcess: Process = new Process();
  editedSurface: Surface = new Surface();
  editedOther: Other = new Other();
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
  rowSelection = "multiple";
  columnApi;
  columnApi_Process;
  columnApi_Surface;
  columnApi_Other;
  public gridOptions: GridOptions;
  //editForm: FormGroup = new FormGroup({});
  constructor(private _formBuilder: FormBuilder,
    private _modalService: NzModalService,
    private _roleService: RoleService,
    private _menuService: MenuService,
    private _messageService: NzMessageService,
    private _srmQotService: SrmQotService,
 

  ) {  
    this.rowData_Material = [];
    this.data1 = [];
    this.rowData_Process = [];
    this.rowData_Surface = [];
    this.rowData_Other = [];
    /**********/
    /*material*/
    this.columnDefs = [
      {
        headerName: "素材材質",
        field: "M_MATERIAL",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
      },
      {
        headerName: "材料單價",
        field: "M_PRICE",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "材料成本價",
        field: "M_COST_PRICE",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "長",
        field: "LENGTH",
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "寬",
        field: "WIDTH",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "高",
        field: "HEIGHT",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "密度",
        field: "DENSITY",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "重量",
        field: "WEIGHT",
        enableRowGroup: true,
        cellClass: "show-cell",
        editable: true,
        width: "150px",
      },
      {
        headerName: "備註",
        field: "NOTE",
        enableRowGroup: true,
        cellClass: "show-cell",
        editable: true,
        width: "150px",
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
        field: "P_PROCESS_NUM",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "250px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
      },
      {
        headerName: "工時(時)",
        field: "P_HOURS",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "單價(時)",
        field: "P_PRICE",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "機台",
        field: "P_MACHINE",
        enableRowGroup: true,
        cellClass: "show-cell",
        autoHeight: true,
        wrapText: true,
      },
      {
        headerName: "備註",
        field: "P_NOTE",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      }
    ]
     /*surface*/
     this.columnDefs_SURFACE = [
      {
        headerName: "工序",
        field: "S_PROCESS",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "250px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
      },
      {
        headerName: "次數",
        field: "S_TIMES",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "單價(時)",
        field: "S_PRICE",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "備註",
        field: "S_NOTE",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      }
    ]
     /*other*/
     this.columnDefs_OTHER = [
      {
        headerName: "項目",
        field: "O_ITEM",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "250px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
      },
      {
        headerName: "說明",
        field: "O_DESCRIPTION",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "單價",
        field: "O_PRICE",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "備註",
        field: "O_NOTE",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      }
    ]
    /**********/
  }

 

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
 
  page: number = 1;
  size: number = 10;
  total: number = 0;
  searchString = null;

  submitEdi() {

  }

  onGridReady(params) {
    function myRowClickedHandler(event) {
    console.log('The row was clicked');
  }
  params.api.addEventListener('rowClicked', myRowClickedHandler);
  this.gridApi = params.api;
  this.columnApi = params.columnApi;
  console.log(params.api);

  //this.gridApi.sizeColumnsToFit();
}

  onGridReady_Surface(params) {
    function myRowClickedHandler(event) {
      console.log('The row was clicked');
    }
    params.api.addEventListener('rowClicked', myRowClickedHandler);
    this.gridApi_Surface = params.api;
    this.columnApi_Surface = params.columnApi;
 
    //this.gridApi_Surface.sizeColumnsToFit();
  }
  
  onGridReady_Process(params) {
    function myRowClickedHandler(event) {
      console.log('The row was clicked');
    }
    params.api.addEventListener('rowClicked', myRowClickedHandler);
    this.gridApi_Process = params.api;
    this.columnApi_Process = params.columnApi;
    console.log(params.api);
    //this.gridApi_Process.sizeColumnsToFit();
  }
  onGridReady_Other(params) {
    function myRowClickedHandler(event) {
      console.log('The row was clicked');
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
    console.log(selectedRows);
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
    console.log(selectedRows);
    //this.gridApi.applyTransaction({ remove: selectedRows });
  }
  deleteSurface() {
    let selectedNodes = this.gridApi_Surface.getSelectedNodes();
    let selectedData = selectedNodes.map(node => node.data);
    let selectedRows = this.gridApi_Surface.getSelectedRows();
    let temp = this.rowData_Surface;
    for (var i = selectedRows.length - 1; i >= 0; i--) {
      for (var j = 0; j < this.rowData_Process.length; j++) {
        if (this.rowData_Surface[j].vendor == selectedRows[i].vendor) {
          temp.splice(j, 1);
          break;
        }
      }
    }
    this.rowData_Surface = temp;
    this.gridApi_Surface.setRowData(this.rowData_Surface);
    console.log(selectedRows);
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
    console.log(selectedRows);
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
  submitProcessList(){
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
        "P_PROCESS_NUM": this.editedProcess.processno,
        "P_HOURS":this.editedProcess.p_hour, 
        "P_PRICE":this.editedProcess.price,
        "P_MACHINE":this.editedProcess.machine,
        "P_NOTE":this.editedProcess.note
      });
      //alert('aaaaa+ '+ this.rowData_Process.value)
      console.log(this.gridApi);
      console.log('-------------');
      console.log(this.gridApi_Process);
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
      this.editedMatetial.totalcost = this.editForm_Material.value['material_totalcost'];
      this.editedMatetial.note = this.editForm_Material.value['material_note'];
      console.info(this.editForm_Material.value);
      //alert('name = '+this.editedMatetial.name)
      /*寫入grid */
      
      this.rowData_Material.push({
        "M_MATERIAL":this.editedMatetial.name,
        "M_PRICE":this.editedMatetial.price, 
        "M_COST_PRICE":this.editedMatetial.cost,
        "LENGTH":this.editedMatetial.length,
        "WIDTH":this.editedMatetial.width,
        "HEIGHT":this.editedMatetial.height,
        "DENSITY":this.editedMatetial.density,
        "M_TOTAL_COST":this.editedMatetial.totalcost,
        "WEIGHT":this.editedMatetial.weight, 
        "NOTE":this.editedMatetial.note,
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
      material_name: [this.editedMatetial.name, [Validators.required]],
      material_price: [this.editedMatetial.price, [Validators.required]],
      material_cost: [this.editedMatetial.cost, [Validators.required ]],
      material_length: [this.editedMatetial.length, [Validators.required]],
      material_width: [this.editedMatetial.width, [Validators.required]],
      material_height: [this.editedMatetial.height, [Validators.required,]],
      material_density: [this.editedMatetial.density, [Validators.required]],
      material_weight: [this.editedMatetial.weight, [Validators.required]],
      material_totalcost: [this.editedMatetial.totalcost, [Validators.required]],
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
  submitOtherList(){ 
    for (const i in this.editForm_Other.controls) {
      this.editForm_Other.controls[i].markAsDirty();
      this.editForm_Other.controls[i].updateValueAndValidity();
    }
    alert(this.editForm_Other.valid)
    if (this.editForm_Other.valid) {
      this.editedOther.item = this.editForm_Other.value['other_item'];
      this.editedOther.price = this.editForm_Other.value['other_price'];
      this.editedOther.note = this.editForm_Other.value['other_note'];
      this.editedOther.description = this.editForm_Other.value['other_desc'];
     
      console.info(this.editForm_Other.value);
      alert('name = '+this.editedOther.item)
      /*寫入grid */
      
      this.rowData_Other.push({
        "O_ITEM":this.editedOther.item,
        "O_PRICE": this.editedOther.price,
        "O_DESCRIPTION":this.editedOther.description,
        "O_NOTE":this.editedOther.note
      });

      this.gridApi_Other.setRowData(this.rowData_Other);
      this.tplModal.close();
    }}

  submitSurfaceList() {
    /**/
    for (const i in this.editForm_Surface.controls) {
      this.editForm_Surface.controls[i].markAsDirty();
      this.editForm_Surface.controls[i].updateValueAndValidity();
    }
    alert(this.editForm_Surface.valid)
    if (this.editForm_Surface.valid) {
      this.editedSurface.process = this.editForm_Surface.value['surface_name'];
      this.editedSurface.price = this.editForm_Surface.value['surface_cost'];
      this.editedSurface.note = this.editForm_Surface.value['surface_note'];
      this.editedSurface.times = this.editForm_Surface.value['surface_times'];
     
     
      console.info(this.editForm_Surface.value);
      alert('name = '+this.editedSurface.process)
      /*寫入grid */
      
      this.rowData_Surface.push({
        "S_PROCESS":this.editedSurface.process,
        "S_TIMES":this.editedSurface.times,
        "S_PRICE":this.editedSurface.price,
        "S_NOTE":this.editedSurface.note
      });

      this.gridApi_Surface.setRowData(this.rowData_Surface);
      this.tplModal.close();
    }
  }
  checktype(number:number){
    if (isNaN(number)) {
      alert("欄位格式錯誤");
      return;
    }
  }
  addSurface(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.editedSurface = new Surface();
    this.editForm_Surface = this._formBuilder.group({
      //material_name: [this.editedMatetial.name, [Validators.required, Validators.maxLength(15)]],
      surface_name: [this.editedSurface.process, [Validators.required]],
      surface_times: [this.editedSurface.times, [Validators.required]],
      surface_cost: [this.editedSurface.price, [Validators.required ]],
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
      other_item: [this.editedOther.item, [Validators.required]],
      other_desc: [this.editedOther.description],
      other_price: [this.editedOther.price, [Validators.required ]],
      other_note: [this.editedOther.note, [Validators.required]]
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
    });
  }
  /*0730*/

  



  addProcess(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.editForm_Process= this._formBuilder.group({
      //material_name: [this.editedMatetial.name, [Validators.required, Validators.maxLength(15)]],
      process_no: [this.editedProcess.processno, [Validators.required]],
      process_cost: [this.editedProcess.price, [Validators.required]],
      process_hour: [this.editedProcess.p_hour, [Validators.required ]],
      process_machine: [this.editedProcess.machine, [Validators.required]],
      process_remark: [this.editedProcess.note],     
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
    });
  }

  /**/
  saveqotmatnr() {
    for (const i in this.editForm_Material.controls) {
      this.editForm_Material.controls[i].markAsDirty();
      this.editForm_Material.controls[i].updateValueAndValidity();
    }
    //alert(this.editForm.valid)
    if (this.editForm_Material.valid) {
      this.editedMatetial.name = this.editForm_Material.value['material_name'];
      alert('aa = '+this.editedMatetial.name)
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
    alert("saveqotmatnr");
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
