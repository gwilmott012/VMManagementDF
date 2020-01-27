import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
    selector: 'app-virtual-machine',
    templateUrl: './virtual-machine.component.html'
})
export class VirtualMachineComponent
{
    public machines: MachineInfo[];
    private elements: HTMLInputElement[] = [];

    constructor(private _http: HttpClient) {
        this.GetMachines();
    }

    GetMachines()
    {
        this._http.get<MachineInfo[]>(document.baseURI + 'virtualmachine').subscribe(result => {
            this.machines = result;
        }, error => console.error(error));
    }

    ToggleMachines(vmid: VirtualMachineId)
    {
        this._http.post(
            'https://vmmanagementdf20200119084109.azurewebsites.net/api/vmManagementOrchestration_Run',
            vmid
        ).subscribe(responseData => {
            console.log(responseData);
        })

        this.UpdateTable();
    }

    UpdateTable()
    {
        let timerId = setInterval(() => {
            this.GetMachines();
        }, 3000)

        setTimeout(() => clearInterval(timerId), 20000);
    }

    ToggleChanged(event: any)
    {
        let checkbox: HTMLInputElement = event.target;

        if (this.elements.includes(checkbox)) {
            this.elements.pop();
        }
        else {
            this.elements.push(checkbox);
        }
    }

    ToggleState()
    {
        let vmid = new VirtualMachineId();

        for (var i = 0; i < this.elements.length; i++)
        {
            vmid.AddId(this.elements[i].id);
            this.elements[i].checked = false;
        }

        this.ToggleMachines(vmid);
    }    
}

interface MachineInfo {
    name: string;
    state: string;
    key: string;
}

export class VirtualMachineId
{
    VirtualMachineIdList: string[] = [''];

    constructor() { this.VirtualMachineIdList }
    
    AddId(id: string)
    {
        this.VirtualMachineIdList.push(id);
    }
}
