import { Component, OnInit, ViewChild, Renderer2 } from '@angular/core';
import { WorkflowGroupTreeComponent } from '../workflow-group-tree/workflow-group-tree.component';
import { WorkflowInstance } from '../model/workflowInstance';
import { WorkFlowForm } from '../model/workflowForm';
import { WorkFlowFormControl } from '../model/workFlowFormControl';
import { WorkflowNode } from '../model/workflowNode';
import { WorkflowLink } from '../model/workflowLink';
import { WorkflowService } from 'src/app/business/workflow/workflow.service';
import { WorkflowFormService } from 'src/app/business/workflow/workflow-form.service';
import { WorkflowFlowService } from 'src/app/business/workflow/workflow-flow.service';
import { WorkflowInstanceService } from 'src/app/business/workflow/workflow-instance.service';
import * as jp from 'jsplumb';
import { WorkFlow } from '../model/workflow';
import { WorkflowInstanceRoute } from '../model/workflowInstanceRoute';
import { StorageService } from 'src/app/services/storage.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-handle-work-flow',
  templateUrl: './handle-work-flow.component.html',
  styleUrls: ['./handle-work-flow.component.less']
})
export class HandleWorkFlowComponent implements OnInit {

  @ViewChild('tree', { static: true })
  workflowGroupTree: WorkflowGroupTreeComponent;

  @ViewChild('formTpl', { static: true })
  _formTpl;

  @ViewChild('flowTpl', { static: true })
  _flowTpl;

  @ViewChild('commentTpl', { static: true })
  _commentTpl;

  @ViewChild('flowRouteTpl', { static: true })
  _flowRouteTpl;

  // 工作流實例數據
  data: WorkflowInstance[] = [];

  // 錶單設計數據
  private _formData: WorkFlowForm = new WorkFlowForm();

  // 錶單控件數據
  formControlList: WorkFlowFormControl[] = [];

  // 節點數據
  nodeDataList: WorkflowNode[] = [];
  linkDataList: WorkflowLink[] = [];

  // 節點處理數據
  routeDataList: WorkflowInstanceRoute[] = [];

  // 控件值
  controlValues: { [key: string]: string; } = {};

  // 評論
  comment;

  page: number = 1;
  size: number = 10;
  total: number = 0;

  wfpage: number = 1;
  wfsize: number = 10;
  wftotal: number = 0;

  _nzModal: NzModalRef;

  checkedData: WorkflowInstance;

  constructor(
    private _storageService: StorageService,
    private _renderer: Renderer2,
    private _modalService: NzModalService,
    private _messageService: NzMessageService,
    private _workflowService: WorkflowService,
    private _formService: WorkflowFormService,
    private _flowService: WorkflowFlowService,
    private _workflowInstanceService: WorkflowInstanceService) { }

  ngOnInit(): void {
    this.refresh();
  }

  refresh() {
    this._workflowInstanceService.getHandledInstance(this.page, this.size)
      .subscribe((result: any) => {
        this.data = result.data;
        this.total = result.count;
      });
  }

  // 檢視內容
  viewForm(data) {
    this.checkedData = data;

    // 取得當前工作流的處理流程
    this._workflowInstanceService.getInstanceRoute(this.checkedData.id).subscribe((result: any) => {
      this.routeDataList = result;
    });

    // 取得錶單以及錶單組件的數據,渲染頁麵
    this._formService.get(data.workFlowId).subscribe((result: any) => {
      this._formData = result.formResult;
      this.formControlList = result.formControlResults;

      // 初始化錶單區域狀態
      //this._renderer.setStyle(this._formArea.nativeElement, 'height', `${this._formData.height}px`);
      //this._renderer.setStyle(this._formArea.nativeElement, 'width', `${this._formData.width}px`);
      //this._renderer.setStyle(this._formArea.nativeElement, 'background-color', this._formData.background);

      this.controlValues = {};
      this._workflowInstanceService.getControlValue(data.id).subscribe((result: any) => {
        result.forEach(element => {
          this.controlValues[element.formControlDomId] = element.value;
        });
      });

      this._nzModal = this._modalService.create({
        nzTitle: '編輯內容',
        nzContent: this._formTpl,
        nzFooter: null,
        nzMaskClosable: false,
        nzWidth: document.body.clientWidth * 0.8
      });

      let ele = document.getElementById('form-area');
      this._renderer.setStyle(ele, 'height', `${this._formData.height}px`);
      this._renderer.setStyle(ele, 'width', `${this._formData.width}px`);
      this._renderer.setStyle(ele, 'background-color', this._formData.background);
    });
  }

  // 檢視流程
  viewflow(data) {
    this.checkedData = data;

    // 取得當前工作流的處理流程
    this._workflowInstanceService.getInstanceRoute(this.checkedData.id).subscribe((result: any) => {
      this.routeDataList = result;
    });

    // 取得工作流的節點和鏈接數據,生成圖形
    this._flowService.get(data.workFlowId).subscribe((result: any) => {
      this.nodeDataList = result.workFlowNodeResults ? result.workFlowNodeResults : [];
      this.linkDataList = result.workFlowLinkResults ? result.workFlowLinkResults : [];

      this._modalService.create({
        nzTitle: '檢視流程',
        nzContent: this._flowTpl,
        nzFooter: null,
        nzMaskClosable: false,
        nzWidth: document.body.clientWidth * 0.8
      });
      let jsPlumbInstance: any = jp.jsPlumb.getInstance({
        DragOptions: { cursor: 'move', zIndex: 2000 },
        Container: 'flowContainer'
      });
      let endpointOption: jp.EndpointOptions = {
        maxConnections: 100,
        reattachConnections: true,
        type: 'Dot',
        connector: 'Flowchart',
        isSource: true,
        isTarget: true,
        paintStyle: { fill: 'transparent', stroke: 'transparent', strokeWidth: 1 },
        connectorStyle: { stroke: 'rgba(102, 96, 255, 0.9)', strokeWidth: 3 },
        connectorOverlays: [["PlainArrow", { location: 1 }]],
      };
      // this.nodeDataList.forEach(node => {
      //   jsPlumbInstance.addEndpoint(node.domId, endpointOption);
      //   jsPlumbInstance.makeTarget(node.domId, {});
      //   jsPlumbInstance.makeSource(node.domId, {});
      // });

      // 因為nz的模態框的創建需要時間,因此對dom操作設定了延時
      setTimeout(() => {
        this.nodeDataList.forEach(node => {

          jsPlumbInstance.makeSource(node.domId, {
            anchor: 'Continuous',
            allowLoopback: false,
            filter: () => {
              return false;
            }
          }, endpointOption);
          jsPlumbInstance.makeTarget(node.domId, {
            anchor: 'Continuous',
            allowLoopback: false,
            filter: () => {
              return false;
            }
          }, endpointOption);
        });
        this.linkDataList.forEach(linkData => {
          jsPlumbInstance.connect({
            source: document.getElementById(linkData.sourceId),
            target: document.getElementById(linkData.targetId),
          });
        });
      }, 400);

    });

  }

  // 是否是當前處理的流程
  canHandle() {
    return this.routeDataList.find(data => data.handlePepleAccount == this._storageService.userName &&
      data.handleState == 1) && this.checkedData.workFlowInstanceState == 2
  }

  cancel() {
    this._nzModal.close();
  }

  pageChange() {
    this.refresh();
  }

  sizeChange() {
    this.page = 1;
    this.refresh();
  }

  checkedWfTypeId;

  wfdata: WorkFlow[] = [];

  nodeChecked(event) {
    if (event) {
      this.checkedWfTypeId = event;
      this.loadWf();
    }
  }

  loadWf() {
    if (this.checkedWfTypeId) {
      this._workflowService.getList(this.wfpage, this.wfsize, this.checkedWfTypeId).subscribe(
        (result: any) => {
          this.wfdata = result.data;
          this.wftotal = result.count;
        }
      )
    }
  }

  wfPageChange() {
    this.loadWf();
  }

  wfSizeChange() {
    this.page = 1;
    this.loadWf();
  }

  // 同意
  approve() {
    this.comment = '';
    this._modalService.confirm({
      nzTitle: '是否同意？',
      nzContent: this._commentTpl,
      nzOnOk: () => {
        this._workflowInstanceService.approveHandledInstance({
          workFlowInstanceId: this.checkedData.id,
          IsPass: true,
          handleComment: this.comment
        }).subscribe(() => {
          this._messageService.success('處理完畢!');
          this.refresh();
          this._nzModal.close();
        });
      }
    });
  }

  // 拒絕
  disApprove() {
    this.comment = '';
    this._modalService.confirm({
      nzTitle: '是否拒絕？',
      nzOnOk: () => {
        this._workflowInstanceService.approveHandledInstance({
          workFlowInstanceId: this.checkedData.id,
          IsPass: false,
          handleComment: this.comment
        }).subscribe(() => {
          this._messageService.success('處理完畢!');
          this.refresh();
          this._nzModal.close();
        });
      }
    });
  }

  // 檢視處理流程
  viewRoutes() {
    this._modalService.create({
      nzTitle: '處理過程',
      nzContent: this._flowRouteTpl,
      nzFooter: null,
      nzMaskClosable: false,
      nzWidth: document.body.clientWidth * 0.8
    });
  }

  getPx(dis) {
    return `${dis}px`;
  }

  getState(state) {
    let result;
    switch (state) {
      case 1:
        result = '未提交';
        break;
      case 2:
        result = '流轉中';
        break;
      case 3:
        result = '已拒絕';
        break;
      case 4:
        result = '已結束';
        break;
      case 5:
        result = '無法進行';
        break;
      case 6:
        result = '已取消';
        break;
    }
    return result;
  }


  getHandleState(state) {
    let result;
    switch (state) {
      case 1:
        result = '未處理';
        break;
      case 2:
        result = '通過';
        break;
      case 3:
        result = '拒絕';
        break;
    }
    return result;

  }

}
