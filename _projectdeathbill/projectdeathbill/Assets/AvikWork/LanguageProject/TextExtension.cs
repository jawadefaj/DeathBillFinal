using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class TextExtension {
	

//	public static void setText(this LanguageManager lm, LanguageManager.string_type type, Text textField){
//		switch(type)
//		{
//			case LanguageManager.string_type.avik:
//				//Debug.Log((int)lm.checkLanguage());
//				if((int)lm.checkLanguage()==0)
//				{
//					//lm.text = lm.lan_dict[LanguageManager.string_type.avik].GetValue(0).ToString();
//					textField.text = lm.lan_dict[LanguageManager.string_type.avik].GetValue(0).ToString();       
//					textField.GetComponent<Text>().font = lm.en_font;
//					textField.GetComponent<Text>().fontSize = lm.en_font_size;
//				}
//				else{
//					textField.text = lm.lan_dict[LanguageManager.string_type.avik].GetValue(1).ToString();       
//					textField.GetComponent<Text>().font = lm.bn_font;
//					textField.GetComponent<Text>().fontSize = lm.bn_font_size;
//				}
//				break;
//			case LanguageManager.string_type.jawad:
//				if((int)lm.checkLanguage()==0)
//				{
//					textField.text = lm.lan_dict[LanguageManager.string_type.jawad].GetValue(0).ToString();       
//					textField.GetComponent<Text>().font = lm.en_font;
//					textField.GetComponent<Text>().fontSize = lm.en_font_size;
//				}
//				else
//				{
//					textField.text = lm.lan_dict[LanguageManager.string_type.jawad].GetValue(1).ToString();       
//					textField.GetComponent<Text>().font = lm.bn_font;
//					textField.GetComponent<Text>().fontSize = lm.bn_font_size;
//				}
//				break;
//
//		}
//	}

   // string txt = "";
	public static void setText(this Text textField, LanguageManager.string_type type)
	{
		//switch (Language.Bangla){
		switch (UserSettings.SelectedLanguage) {
		case Language.Bangla:
			textField.GetComponent<Text>().font = LanguageManager.instance.lan_dict[type].bn_font;
			textField.GetComponent<Text>().fontSize = LanguageManager.instance.lan_dict[type].bn_font_size;
			textField.text = LanguageManager.instance.lan_dict[type].bn_name;
			break;
		case Language.English:
			textField.GetComponent<Text>().font = LanguageManager.instance.lan_dict[type].en_font;
			textField.GetComponent<Text>().fontSize = LanguageManager.instance.lan_dict[type].en_font_size;
			textField.text = LanguageManager.instance.lan_dict[type].en_name;
			break;
		}
	}

}