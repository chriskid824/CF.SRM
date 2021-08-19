import { Component, OnInit,ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SrmDeliveryService } from '../../../business/srm/srm-delivery.service';
import { ActivatedRoute } from '@angular/router';
import { ZXingScannerComponent } from '@zxing/ngx-scanner';
import { stringify } from '@angular/compiler/src/util';
@Component({
  selector: 'app-delivery-receive',
  templateUrl: './delivery-receive.component.html',
  styleUrls: ['./delivery-receive.component.less']
})
export class DeliveryReceiveComponent implements OnInit {
  @ViewChild('scanner')
  scanner: ZXingScannerComponent;
  searchForm: FormGroup = new FormGroup({});
  rowData;
  gridApi;
  gridColumnApi;
  deliveryLId;
  deliveryNum="";
  availableDevices: MediaDeviceInfo[];
  selectedDevice: MediaDeviceInfo;
  hasCameras = false;
  hasPermission: boolean;
  isChecked=true;
  isVisible = false;
  constructor(private route: ActivatedRoute,private _formBuilder: FormBuilder,private _srmDeliveryService: SrmDeliveryService) { }

  ngOnInit(): void {
    const routeParams = this.route.snapshot.paramMap;
    this.deliveryLId = routeParams.get('deliveryLId');
    this.deliveryNum = routeParams.get('deliveryNum');
    this.searchForm = this._formBuilder.group({
      DELIVERY_NUM: this.deliveryNum,
      DELIVERY_L_ID: this.deliveryLId,
    });
    this.scanner.camerasFound.subscribe((devices: MediaDeviceInfo[]) => {
      this.hasCameras = true;
      console.log('Devices: ', devices);
      this.availableDevices = devices;
  });

  this.scanner.camerasNotFound.subscribe((devices: MediaDeviceInfo[]) => {
      console.error('An error has occurred when trying to enumerate your video-stream-enabled devices.');
  });

  this.scanner.permissionResponse.subscribe((answer: boolean) => {
    this.hasPermission = answer;
  });
  }
  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.getDelyveryList(null);
    // this._srmPoService.GetPo(null)
    //   .subscribe((data) => {
    //     this.rowData = data;
    //   });
  }
  submitSearch() {
    this.refresh();
  }
  pageChange() {
    this.refresh();
  }
  refresh() {
    var query = {
      deliveryNum: this.searchForm.value["DELIVERY_NUM"] == null ? "" : this.searchForm.value["DELIVERY_NUM"],
      //deliveryLId: this.searchForm.value["DELIVERY_L_ID"] == null ? "0" : this.searchForm.value["DELIVERY_L_ID"],
      deliveryLId:this.deliveryLId,
    }
    this.getDelyveryList(query);
  }
  getDelyveryList(query){

    if(query==null)
    {
      query = {
        deliveryNum:this.deliveryNum,
        deliveryLId:this.deliveryLId,
      }
    }
    this._srmDeliveryService.GetDeliveryL(query)
      .subscribe((result) => {
        console.info(result);
        this.rowData = result[0].SrmDeliveryLs;
        this.deliveryNum=result[0].DeliveryNum;
      });
  }

  camerasNotFound(e: Event) {
    // Display an alert modal here
  }

  cameraFound(e: Event) {
    // Log to see if the camera was found
  }

  handleQrCodeResult(result: string) {
    if(result.indexOf("/")>-1) {
      var DeliveryId=result.split('/')[0];
      var DeliveryLId=result.split('/')[1];

      if(this.deliveryNum!="")
      {
        if(this.rowData.find(p=>p.DeliveryId==DeliveryId&&p.DeliveryLId==DeliveryLId)){alert("此出貨項次已存在!請選擇其他項次"); return;}
        else
        {
          alert("此出貨項次不是該出貨單請重新選擇其他出貨項次");
        }
      }


      else
      {

      }
    }
    else
    {
      this.deliveryNum=result;
       this.searchForm = this._formBuilder.group({
       DELIVERY_NUM: this.deliveryNum,
       DELIVERY_L_ID: this.deliveryLId,
     });
    this.refresh();

    }
  }

  qrResultString: string;

  clearResult(): void {
    this.qrResultString = null;
  }

  onCodeResult(resultString: string) {
    this.qrResultString = resultString;
  }

  camerasFoundHandler(devices: MediaDeviceInfo[]): void {
    this.availableDevices = devices;
    this.hasCameras = Boolean(devices && devices.length);
    if(this.hasCameras)
    {
      this.selectedDevice= this.availableDevices[0];
    }

  }
  onDeviceSelectChange(selectedValue: string) {
    console.log('Selection changed: ', selectedValue);
    //this.selectedDevice = this.scanner.getAnyVideoDevice();
}
showModal(): void {
  this.isVisible = true;
}

handleOk(): void {
  console.log('Button ok clicked!');
  this.isVisible = false;
}

handleCancel(): void {
  console.log('Button cancel clicked!');
  this.isVisible = false;
}
}
