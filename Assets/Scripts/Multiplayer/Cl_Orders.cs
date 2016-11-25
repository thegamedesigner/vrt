﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cl_Orders : MonoBehaviour
{
	public enum MouseOrders { None, SelectionBox_StartDragging, SelectionBox_StopDragging, DraggingObj_Start, DraggingObj_Stop, End }
	public static int draggingObj = -1;
	public static List<ObjFuncs.Obj> gridObjs;//Objs that the dragged object is checking against their grids

	public static void InputOrders()//Called every frame from hl.HlObj Update
	{
		//There is a generic way to do this, but for now, I'm just going to handle each case

		//Make sure that the connection isn't null
		if (hl.hlObj == null) { return; }

		//Get mouse pos in the world
		Vector3 mousePos = Vector3.zero;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 999, xa.de.toHitFloorMask))
		{
			mousePos = hit.point;
		}

		MouseOrders mouseOrder = MouseOrders.None;
		if (Input.GetMouseButtonDown(0))//Left click - Drag selection 
		{
			//start dragging a selection box
			mouseOrder = MouseOrders.SelectionBox_StartDragging;
		}
		if (Input.GetMouseButtonUp(0))//Left click up, release dragged object or stop selection box?
		{
			//I wasn't dragging an obj, so I must have been dragging a selection box
			mouseOrder = MouseOrders.SelectionBox_StopDragging;
		}
		SelectionScript.HandleSelection(mouseOrder);//Handles local selection box

		//Issue a move command to all selected units of the correct player
		if (Input.GetMouseButtonDown(1))//Move order
		{
			//Send the order to the server. Fire and forget!
			ObjFuncs.Obj o;

			//send one order per unit ordered.
			List<int> result = new List<int>();
			for (int i = 0; i < ObjFuncs.objs.Count; i++)
			{
				o = ObjFuncs.objs[i];
				if (o.isUnit && o.ownedByClId == hl.local_uId && o.selected)
				{
					result.Add(o.uId);
				}
			}
			int[] unitIds = new int[result.Count];
			for (int i = 0; i < result.Count; i++) { unitIds[i] = result[i]; }//write to the array

			hl.hlObj.CmdMoveOrder(hl.local_uId, mousePos, unitIds);

			//Trigger local effect
			Effects.MoveOrderEffect(mousePos);
		}

		//This is currently hardwired to specific keys, but later players will be able to rebind.
		if (Input.GetKeyDown(KeyCode.Alpha1))//Create a command pod!
		{
			ChatScript.ChatLocally("Detected local createObj order: Marine");
			//Send the order to the server
			hl.hlObj.CmdCreateObj(hl.local_uId, mousePos, (int)ObjFuncs.Type.Marine);

			//Trigger local effect
			//Effects.CircleBlip(mousePos, 5, 5);
		}
	}
}
