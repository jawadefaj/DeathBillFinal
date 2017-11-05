using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

	public Font eng_font;
	public Font ban_font;
//
//	public int bn_font_size;
//	public int en_font_size;
	
	//public Text btnText;
	private string txt,btnTxt;
	//public string text;
	public static LanguageManager instance;
	//public Text textField1;
	//public Text textField2;

	private const int en_font_baseSize = 52;
	private const int en_nameFont_baseSize = 55;

	void Awake()
	{
		instance = this;

        #region names
        lan_dict.Add(string_type.kabir, new TextProperties() 
			{ bn_font = ban_font, en_font = eng_font,bn_font_size = 40, en_font_size = en_nameFont_baseSize, bn_name = "Kwei", en_name = "Kabir"});
		
        lan_dict.Add(string_type.shamsu, new TextProperties() 
			{ bn_font = ban_font,en_font = eng_font,bn_font_size = 40, en_font_size =  en_nameFont_baseSize, bn_name = "kvgmy", en_name = "Shamsu"});

        lan_dict.Add(string_type.taposh, new TextProperties() 
			{ bn_font = ban_font,en_font = eng_font,bn_font_size = 40, en_font_size =  en_nameFont_baseSize, bn_name = "Zvcm", en_name = "Taposh"});

        lan_dict.Add(string_type.anila, new TextProperties() 
			{ bn_font = ban_font,en_font = eng_font,bn_font_size = 40, en_font_size =  en_nameFont_baseSize, bn_name = "Awbjv", en_name = "Anila"});

        lan_dict.Add(string_type.bodi, new TextProperties() 
			{ bn_font = ban_font,en_font = eng_font,bn_font_size = 40, en_font_size =  en_nameFont_baseSize, bn_name = "ew`", en_name = "Bodi"});
        #endregion
        //GLv‡b ‡_‡K ‡gBj Pv‡iK `~‡i GKUv 
        #region scene1
        lan_dict.Add(string_type.scene1_samsu1_part0, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "mevB ‡kvb| ", en_name = "Listen up guys..."});

        lan_dict.Add(string_type.scene1_samsu1_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "GLv‡b ‡_‡K gvBj Pv‡iK `~‡i ", en_name = "About four miles ahead of here"});

        lan_dict.Add(string_type.scene1_samsu1_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "GKUv K¨v‡¤ú nvbv`viiv ", en_name = "a military camp is holding captive"});

        lan_dict.Add(string_type.scene1_samsu1_part3, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "A‡bK ‡g‡q‡K AvU‡K ‡i‡L‡Q|  ", en_name = "a large group of women..."});

        lan_dict.Add(string_type.scene1_samsu2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "ûgg| ‡kvb mevB|", en_name = "AAAAAAAAAAAAAAAAAA"});

        lan_dict.Add(string_type.scene1_kabir_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
				bn_name = "kvgmy fvB Avgiv eBmv AvwQ K¨vb GL‡bv ...", en_name = "Shamsu vai, what are we waiting for?"});

        lan_dict.Add(string_type.scene1_kabir_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
				bn_name = "P‡jb evBi nBqv cwo|", en_name = "Lets get going!"});

        lan_dict.Add(string_type.scene1_taposh_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "gv_v Mig KB‡ib bv Kwei fvB| ", en_name = "Calm down Kabir vai..."});

        lan_dict.Add(string_type.scene1_taposh_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "c­¨vbUv ïBbv ‡bB mevB Av‡M| ", en_name = "Lets hear the plan out first."});
        #endregion
      
        #region scene2
        lan_dict.Add(string_type.scene2_samsu1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "K¨v‡¤ú wgwjUvwii cvnviv A‡bK Kov| ", en_name = "The camp is heavily guarded."});

        lan_dict.Add(string_type.scene2_samsu2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Avgv‡`i wZbw`K ‡_‡K Avµgb Ki‡Z n‡e, ", en_name = "We will attack from three sides"});

        lan_dict.Add(string_type.scene2_samsu3, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "hv‡Z Iiv ¸wQ‡q IVvi mgq bv cvq| ", en_name = "We won't allow them to get organized"});

        lan_dict.Add(string_type.scene2_samsu4, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "K¨v‡¤úi Qv‡` ‡gwkb Mvbvi Av‡Q GKUv| ", en_name = "There's a machine gunner on this rooftop."});

        lan_dict.Add(string_type.scene2_samsu5, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Avwg cwðg ‡_‡K Xy‡K IUv‡K gvie|", en_name = "I'll take it out moving in from the west."});
        

        lan_dict.Add(string_type.scene2_samsu6, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "wmMbvj ‡c‡j Kwei Gcvk ‡_‡K XyKwe| ", en_name = "Kabir will clear this path at my signal"});

        lan_dict.Add(string_type.scene2_samsu7, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Kwe‡ii cvk wK¬qvi n‡j Zvcm ", en_name = "Once Kabir clears the extraction route"});

        lan_dict.Add(string_type.scene2_samsu8, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "K¨v‡¤úi `w¶‡Y cwRkb wbwe|", en_name = "Taposh will take position in the south"});

        lan_dict.Add(string_type.scene2_samsu9, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "c‡_ ‡h KqUv‡K cvwe, ", en_name = "and cover us as we move in to evacuate..."});

        lan_dict.Add(string_type.scene2_samsu10, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "GKUvI hv‡Z evuP‡Z bv cv‡i| ", en_name = "Kill all hostiles at sight!"});

        lan_dict.Add(string_type.scene2_samsu11, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Rq evsjv|", en_name = "Joy Bangla!"});
        #endregion

        #region scene4
        lan_dict.Add(string_type.scene4_samsu1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Avcbviv fq cv‡eb bv| ", 
            en_name = "Don't be afraid!"});
        lan_dict.Add(string_type.scene4_samsu2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " Avgiv gyw³evwnbx|  ", 
            en_name = "We are Freedom Fighters!"});
        lan_dict.Add(string_type.scene4_samsu3, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " Avcbv‡`i D×vi Ki‡Z G‡mwQ|", 
            en_name = "Follow my lead..."});
        lan_dict.Add(string_type.scene4_samsu4, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Avgvi mv‡_ ‡ei n‡q Av‡mb| ", 
            en_name = "I'll take you out of here."});
        #endregion

        #region scene5
        lan_dict.Add(string_type.scene5_samsu, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Avcbviv ZvovZvwo cvjvb|", en_name = "Enmey reinforcements!!!"});


//        lan_dict.Add(string_type.scene5_kabir, new TextProperties() 
//        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
//            bn_name = "Avcwb I‡`i‡K wb‡q hvb kvgmy fvB| Avwg Avi Zvcm Kfvi w`‡ZwQ| ", en_name = "Shamsu bro,take them away quickly." +
//                " Me and Taposh are giving cover."});

        lan_dict.Add(string_type.scene5_kabir_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Avcwb I‡`i‡K wb‡q hvb kvgmy fvB| ", en_name = "Shamsu vai! You escort them to safety..."});


        lan_dict.Add(string_type.scene5_kabir_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " Avwg Avi Zvcm Kfvi w`‡ZwQ| ", en_name = "Taposh and I will cover your back."});
        #endregion

        #region scene6
        lan_dict.Add(string_type.scene6_kabir1_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "kvgmy fvB!! ew` ...KB Avc‡biv!!", en_name = "Shamsu vai!! Bodi!! where are you guyz?"});

        lan_dict.Add(string_type.scene6_kabir1_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "ZvcBm¨v ¸wj LvB‡m|", en_name = "TAPOSH IS HIT!!!"});

        lan_dict.Add(string_type.scene6_kabir2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "GB ‡g‡q KB‡Ë Avm‡Q ?", en_name = "Who the hell is this girl?!"});

        lan_dict.Add(string_type.scene6_anila1_part0, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
				bn_name = "‡Kv_vq, ‡`wL ‡`wL| ", en_name = "Where! Let me check..."});

        lan_dict.Add(string_type.scene6_anila1_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
				bn_name = "In, ‡d¬k DÛ| ", en_name =  "Oh! Flesh wound."});

        lan_dict.Add(string_type.scene6_anila1_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
				bn_name = "e¨v‡ÛR ‡eu‡a K‡qKw`b ", en_name = "All it needs is bandages"});

        lan_dict.Add(string_type.scene6_anila1_part3, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
				bn_name = " ‡i÷ wb‡jB ‡m‡i hv‡eb|", en_name = "and a few days of rest..."});

        lan_dict.Add(string_type.scene6_anila2_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
				bn_name = "Avwg Awbjv| cwiPq c‡i w`w”Q|", en_name = "It's Anila. Let's skip the chit-chat for now."});

        lan_dict.Add(string_type.scene6_anila2_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Av‡M e¨v‡ÛR Kivi Kvco w`b|", en_name = "I'll need some clean cloth for dressing..."});
        
        #endregion

        #region scene7
        lan_dict.Add(string_type.scene7_shamsu1_part0, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "‡i÷ ‡bqvi mgq bvB|", 
            en_name = "There's no time for rest."});

        lan_dict.Add(string_type.scene7_shamsu1_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "GKUv e«xR Dov‡Z n‡e|", 
            en_name = "We have to take out a bridge."});
        
        lan_dict.Add(string_type.scene7_shamsu1_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "GUvi Dci w`‡q GKUv cvwK Kbfq hv‡e|", 
            en_name ="An enemy convoy is enroute..."});
        
        lan_dict.Add(string_type.scene7_shamsu1_part3, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " I‡`i AvUKv‡Z bv cvi‡j", 
            en_name = "If we cant stop it"});

        lan_dict.Add(string_type.scene7_shamsu1_part4, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " GB ‡m±‡ii Ab¨vb¨‡`i Acv‡ikb¸‡jv ", 
            en_name = "Other operations in this area"});

        lan_dict.Add(string_type.scene7_shamsu1_part5, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Sv‡gjvi g‡a¨ c‡o hv‡e| ", 
            en_name = "will be jeopardized!!"});

        lan_dict.Add(string_type.scene7_bodi1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Zvigv‡b ‡Zv ‡evgvevwR n‡e|", 
            en_name = "Does that mean explosives?"});

        lan_dict.Add(string_type.scene7_bodi2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Lye gRv n‡e wKš‘|", 
            en_name = "This should be fun!"});

        lan_dict.Add(string_type.scene7_kabir1_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "wKš‘... Zvcm ‡Zv", 
            en_name = "What about Taposh?"});

        lan_dict.Add(string_type.scene7_kabir1_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "¸wj LvBqv weQvbvq cBov Av‡Q|", 
            en_name = "He can barely move!"});

        lan_dict.Add(string_type.scene7_kabir1_part3, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Avgiv wZbR‡bB hvgy ?", 
            en_name = "So it's only three of us?"});

        lan_dict.Add(string_type.scene7_anila1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "wZbRb ‡Kb ? AvwgI AvwQ|", 
            en_name = "I can make it four!"});

        lan_dict.Add(string_type.scene7_kabir2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " wK Kq GB gvBqv ?", 
            en_name = "Is she out of her mind?"});
        
        lan_dict.Add(string_type.scene7_anila2_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " ‡Kb ? Avgvi A¯¿ Pvjv‡bvi ‡U«wbs Av‡Q| ", 
            en_name = "I do have militia training..."});

        lan_dict.Add(string_type.scene7_anila2_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " ‡g‡q‡`i GKUv ‡Mwijv `j AiMvbvBR Kivi ", 
            en_name = "Got caught organizing a guerilla group."});

        lan_dict.Add(string_type.scene7_anila2_part3, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "  mgq Avwg nvbv`vi‡`i nv‡Z aiv cwo| ", 
            en_name = "Besides you are short on crew!"});
        

        lan_dict.Add(string_type.scene7_shamsu2_part1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Av”Qv, wVK Av‡Q|", 
            en_name = "Fine! So be it!"});

        lan_dict.Add(string_type.scene7_shamsu2_part2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Avcwb Avm‡Z cv‡ib Avgv‡`i mv‡_| ", 
            en_name = "But you better know"});

        lan_dict.Add(string_type.scene7_shamsu2_part3, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "wKš‘ ‡g‡q e‡j Avjv`v LvwZi cv‡eb bv|", 
            en_name = "what you are signing up for..."});
        #endregion

        #region scene9
        lan_dict.Add(string_type.scene9_kabir, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "ivRvKvi!! Giv ‡Zv _vKvi K_v AvwQj bv|", en_name = "Rajakars! We didnt expect this!"});

        lan_dict.Add(string_type.scene9_bodi1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "GLb Kx Ki‡ev ?", en_name = "What now Shamsu Vai?"});
        
        lan_dict.Add(string_type.scene9_bodi2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "hvIqvi iv¯—v ‡Zv ‡Kej GKUvB|", en_name = "There's no way around..."});

        lan_dict.Add(string_type.scene9_shamsu1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "Dcvq bvB, GB¸‡jv‡K PycPvc gvi‡Z n‡e| ", en_name = "We must kill them silently."});

        lan_dict.Add(string_type.scene9_shamsu2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "ew` ZyB G·‡c­vwmf ‡hvMvo Ki‡Z hv| ", en_name = "Bodi go get explosives."});

        lan_dict.Add(string_type.scene9_shamsu3, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " evKxiv Avgvi mv‡_ Avq| ", en_name = "Rest of you come with me."});
        
        #endregion

        #region scene 10
        lan_dict.Add(string_type.scene10_shamsu1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "cvK Avwg© ‡Ui ‡c‡q ‡M‡Q|", en_name = "The military has been alerted!"});

        lan_dict.Add(string_type.scene10_shamsu2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "GLb Avgv‡`i mvgbv mvgwb hy‡× bvg‡ZB n‡e|", en_name = "We can't avoid engaging them anymore..."});
        #endregion

        #region scene 11
        lan_dict.Add(string_type.scene11_shamsu1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " A‡bK ‡`wi n‡q ‡M‡Q|", en_name = "It's getting too late."});

        lan_dict.Add(string_type.scene11_shamsu2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "ew`, ZvovZvwo ‡evgv wdU K‡i Avq|", en_name = "Bodi, go set the explosives up!"});

        lan_dict.Add(string_type.scene11_bodi, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "wVK Av‡Q fvB, Avwg hvB‡ZwQ|", en_name = "Right away, Boss!"});

        #endregion

        #region scene 12

        lan_dict.Add(string_type.scene12_kabir, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = "cvB°viv AvBmv co‡Q| ", en_name = "THE CONVOY IS HERE!"});

        lan_dict.Add(string_type.scene12_shamsu1, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
				bn_name = "GLb wcQ‡b wdiv hv‡e bv| ", en_name = "We must buy Bodi time..."});

        lan_dict.Add(string_type.scene12_shamsu2, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
							bn_name = " gvi‡ev bq gie| ", en_name = "Dont let them reach the bridge!"});

        lan_dict.Add(string_type.scene12_anila, new TextProperties() 
        { bn_font = ban_font,en_font = eng_font,bn_font_size = 60, en_font_size = en_font_baseSize, 
            bn_name = " gi‡j mevB GKmv‡_B gie| ", en_name = "Not while we are still breathing..."});
        #endregion

	}

	public enum string_type
	{
		kabir,
		shamsu,
        taposh,
        anila,
        bodi,
        scene1_samsu1_part0,
        scene1_samsu1_part1,
        scene1_samsu1_part2,
        scene1_samsu1_part3,
        scene1_kabir_part1,
        scene1_kabir_part2,
        scene1_taposh_part1,
        scene1_taposh_part2,
        scene1_samsu2,
        scene1_kabir,
        scene1_taposh,
        scene2_samsu1,
        scene2_samsu2,
        scene2_samsu3,
        scene2_samsu4,
        scene2_samsu5,
        scene2_samsu6,
        scene2_samsu7,
        scene2_samsu8,
        scene2_samsu9,
        scene2_samsu10,
        scene2_samsu11,
        scene4_samsu1,
        scene4_samsu2,
        scene4_samsu3,
        scene4_samsu4,
        scene5_samsu,
        scene5_kabir_part1,
        scene5_kabir_part2,
        scene6_kabir1_part1,
        scene6_kabir1_part2,
        scene6_kabir2,
        scene6_anila1_part0,
        scene6_anila1_part1,
        scene6_anila1_part2,
        scene6_anila1_part3,
        scene6_anila2_part1,
        scene6_anila2_part2,
        scene7_shamsu1_part0,
        scene7_shamsu1_part1,
        scene7_shamsu1_part2,
        scene7_shamsu1_part3,
        scene7_shamsu1_part4,
        scene7_shamsu1_part5,
        scene7_shamsu2_part1,
        scene7_shamsu2_part2,
        scene7_shamsu2_part3,
        scene7_anila1,
        scene7_anila2_part1,
        scene7_anila2_part2,
        scene7_anila2_part3,
        scene7_kabir1_part1,
        scene7_kabir1_part2,
        scene7_kabir1_part3,
        scene7_kabir2,
        scene7_bodi1,
        scene7_bodi2,
        scene9_kabir,
        scene9_bodi1,
        scene9_bodi2,
        scene9_shamsu1,
        scene9_shamsu2,
        scene9_shamsu3,
        scene10_shamsu1,
        scene10_shamsu2,
        scene11_shamsu1,
        scene11_shamsu2,
        scene11_bodi,
        scene12_kabir,
        scene12_shamsu1,
        scene12_shamsu2,
        scene12_anila,
       

	}

	public enum language{
		English,
		Bangla
	}

	public Dictionary<string_type, TextProperties> lan_dict = new Dictionary<string_type, TextProperties>();


	int lan;

	// Use this for initialization
	void Start () {
	//	ConfigureText();
	}

	// Update is called once per frame
	void Update () {

    }

//	public void onClickLanguage()
//	{
//		if(btnText.text.ToString()=="English")
//		{
//			btnText.text = "Bangla";
//		}
//		else{
//			btnText.text = "English";
//		}
//
//		ConfigureText();
//	}
//
//	private void ConfigureText()
//	{
//		textField1.setText(string_type.avik);
//		textField2.setText(string_type.jawad);
//	}

	public language checkLanguage()
	{
//		if(btnText.text.ToString()=="Bangla")
//		{
//			return language.English;
//		}
//		else{
            return language.Bangla;
     //   }



	}
	

}
