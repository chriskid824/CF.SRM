import {Component, Inject,OnInit} from '@angular/core';
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { SrmPoService } from '../../../business/srm/srm-po.service';
export interface DialogData {
  PoNum: string;
  PoLId: string;
}
@Component({
  selector: 'app-delivery-add',
  templateUrl: './delivery-add.component.html',
  styleUrls: ['./delivery-add.component.less']
})
export class DeliveryAddComponent implements OnInit{
  rowData;
  PoNumOption = [
    // { label: 'Jack', value: 'jack' },
    // { label: 'Lucy', value: 'lucy' },
    // { label: 'disabled', value: 'disabled', disabled: true }
  ];
  PoLIdOption=[];
  constructor(
    public dialogRef: MatDialogRef<DeliveryAddComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,private _srmPoService: SrmPoService) {
    }
    ngOnInit(): void {
this.getPoLList(null);
    }
  onNoClick(): void {
    this.dialogRef.close();
  }
  getPoLList(query){
    if(query==null)
    {
      query = {
        poNum: "",
        status: "15",
        replyDeliveryDate_s: null,
        replyDeliveryDate_e: null,
      }
    }
    this._srmPoService.GetPoL(query)
      .subscribe((result) => {
        this.rowData=result;
        var newoptions=[];
        for (var po in result) {
          newoptions.push({label:result[po].PoNum,value:result[po].PoNum});
          }
          this.PoNumOption=newoptions;
        //this.rowData = result;
      });
  }
  onChange(value) {
    var poLIdList=this.rowData.filter(p=>p.PoNum==value);
    var newoptions2=[];
    for(var poL in poLIdList)
    {
      newoptions2.push({label:poLIdList[poL].PoLId.toString(),value:poLIdList[poL].PoLId.toString()});
    }
    this.PoLIdOption=newoptions2;
    console.info(this.PoLIdOption);
  }
}
