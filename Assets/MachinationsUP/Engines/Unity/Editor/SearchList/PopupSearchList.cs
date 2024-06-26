﻿using System;
using System.Collections.Generic;
using MachinationsUP.Engines.Unity.Editor;
using UnityEngine;
using UnityEditor;

namespace MachinationsUP.Engines.Unity.Editor
{

  public class PopupSearchList : PopupWindowContent
  {

    public delegate void OnButtonPressed_Handler(List<SearchListItem> selectedItems);
    static public event OnButtonPressed_Handler OnButtonPressed;

    /// <summary>
    /// Current scroll position.
    /// </summary>
    Vector2 scrollPos;

    /// <summary>
    /// Used to grab the position of the last control before the Scroll View, so that the Scroll View
    /// is almost as large as the window.
    /// </summary>
    Rect lastRectBeforeScrollView;

    /// <summary>
    /// Current filter value.
    /// </summary>
    private string _filterValue = "";

    /// <summary>
    /// Width of the popup.
    /// </summary>
    private int _width;

    /// <summary>
    /// Height of the popup.
    /// </summary>
    private int _height;

    /// <summary>
    /// Items to be listed and searched after.
    /// </summary>
    private List<SearchListItem> _items;

    /// <summary>
    /// Tags to be used to filter items.
    /// </summary>
    private List<ITagProvider> _tags;

    private bool _someListItemsHaveTagsWithImages;
    private int _maxListItemTagWidth = -1;
    private int _maxListItemTagHeight = -1;

    private bool _someTagsHaveImages;
    private int _maxTagWidth = -1;
    private int _maxTagHeight = -1;

    /// <summary>
    /// Creates the popup for a Search List.
    /// </summary>
    /// <param name="width">What width should it have.</param>
    /// <param name="items">A list of items.</param>
    /// <param name="tags">List of Tags that can be used to search Items. Each Item should be assigned one of the Tags if this functionality is to work.</param>
    public PopupSearchList(int width, List<SearchListItem> items, List<ITagProvider> tags = null)
    {
      _width = width;
      _height = items.Count == 0 ? 70 : 450;
      _items = items;
      _tags = tags;

      //Check if any of the provided Items has a tag with an image. In that case, we'll have to check for the largest image.
      foreach (SearchListItem item in _items)
        if (item.TagProvider != null && item.TagProvider.Aspect != null)
        {
          _someListItemsHaveTagsWithImages = true;
          if (_maxListItemTagWidth < item.TagProvider.Aspect.width) _maxListItemTagWidth = item.TagProvider.Aspect.width;
          if (_maxListItemTagHeight < item.TagProvider.Aspect.height) _maxListItemTagHeight = item.TagProvider.Aspect.height;
        }

      //Set up tags.
      if (tags != null)
      {
        //Remove all tags that aren't used by any items.
        for (int i = _tags.Count - 1; i >= 0; i--)
        {
          ITagProvider tag = _tags[i];
          bool someItemHasThisTag = false;
          //Check if any of the items have this tag.
          foreach (SearchListItem item in _items)
            if (item.TagProvider == tag)
            {
              someItemHasThisTag = true;
              break;
            }
          if (!someItemHasThisTag) _tags.Remove(tag);
        }

        //Determine max width/height for a tag image.
        foreach (ITagProvider tag in _tags)
          if (tag.Aspect != null)
          {
            _someTagsHaveImages = true;
            if (_maxTagWidth < tag.Aspect.width) _maxTagWidth = tag.Aspect.width;
            if (_maxTagHeight < tag.Aspect.height) _maxTagHeight = tag.Aspect.height;
          }
      }
    }

    override public Vector2 GetWindowSize()
    {
      return new Vector2(_width, _height);
    }

    override public void OnGUI(Rect rect)
    {
      //@@@@@@@@@@@@@@@@@@@@@@@
      //Show notice if no items there.
      if (_items.Count == 0)
      {
        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("No elements here :(. Either your diagram is empty, or there's some association problem.",
            EditorStyles.boldLabel);
        return;
      }

      //@@@@@@@@@@@@@@@@@@@@@@@
      //Title.
      EditorGUILayout.LabelField("Code Generator", EditorStyles.boldLabel);
      EditorGUILayout.BeginHorizontal();

      //@@@@@@@@@@@@@@@@@@@@@@@
      //Set general UI properties.
      EditorGUIUtility.labelWidth = 70;

      //@@@@@@@@@@@@@@@@@@@@@@@
      //Place the "Search" text field.
      _filterValue = EditorGUILayout.TextField("Search:", _filterValue);
      if (GUILayout.Button("Generate", GUILayout.Width(100)))
      {
        Debug.Log("Generating!");
        //Send selected items via event.
        var selectedItems = new List<SearchListItem>();
        foreach (SearchListItem sli in _items)
          if (sli.Checked) selectedItems.Add(sli);
        OnButtonPressed?.Invoke(selectedItems);
        editorWindow.Close();
      }

      EditorGUILayout.EndHorizontal();
      DrawLineInInspector(Color.black, 2, 10);

      //@@@@@@@@@@@@@@@@@@@@@@@
      //Create Tag Buttons.
      EditorGUILayout.BeginHorizontal();
      int buttonsPerRow = (int)Math.Floor((double)Screen.width / 65);
      int currentRow = 0;
      int currentButton = 1;
      //Create Tag filter buttons.
      foreach (ITagProvider node in _tags)
      {
        node.Checked = EditorGUILayout.Toggle(node.Checked, GUILayout.MaxWidth(15),
            GUILayout.Height(_someTagsHaveImages ? _maxTagHeight : 15));
        GUIContent buttonContent = new GUIContent(node.Aspect);
        buttonContent.text = "";
        buttonContent.tooltip = (node.Checked ? "Un-select" : "Select") + " " + node.Name + "s";
        if (GUILayout.Button(buttonContent, GUILayout.MaxWidth(30), GUILayout.Height(30)))
        {
          node.Checked = !node.Checked;
        }

        EditorGUILayout.Space(5);

        //Exceeded number of buttons per row?
        if (currentButton++ > buttonsPerRow)
        {
          //Align everything to left.
          GUILayout.FlexibleSpace();
          //New row;
          EditorGUILayout.EndHorizontal();
          EditorGUILayout.BeginHorizontal();
          currentButton = 1;
          currentRow++;
        }
      }

      GUILayout.FlexibleSpace();
      EditorGUILayout.EndHorizontal();

      //@@@@@@@@@@@@@@@@@@@@@@@
      //Was any Tag filtering done?
      bool anyTagChecked = false;
      foreach (SearchListItem item in _items)
        //More items may have the same TagProvider, and its Checked property is controlled above, in node.Checked.
        if (item.TagProvider != null && item.TagProvider.Checked)
        {
          anyTagChecked = true;
          break;
        }

      //@@@@@@@@@@@@@@@@@@@@@@@
      //Mass-selection buttons.

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Select:", GUILayout.Width(69));
      if (GUILayout.Button("All", GUILayout.Width(70)))
      {
        foreach (SearchListItem item in _items)
          if (ShouldBeVisible(item, anyTagChecked))
            item.Checked = true;
      }

      if (GUILayout.Button("None", GUILayout.Width(70)))
      {
        foreach (SearchListItem item in _items)
          if (ShouldBeVisible(item, anyTagChecked))
            item.Checked = false;
      }

      if (GUILayout.Button("Inverse", GUILayout.Width(70)))
      {
        foreach (SearchListItem item in _items)
          if (ShouldBeVisible(item, anyTagChecked))
            item.Checked = !item.Checked;
      }

      EditorGUILayout.EndHorizontal();

      //@@@@@@@@@@@@@@@@@@@@@@@
      //List of items.

      //Since the above UI element the last before the Scroll View, get its rect. We'll need it to determine scroll view height.
      if (Event.current.type == EventType.Repaint) lastRectBeforeScrollView = GUILayoutUtility.GetLastRect();
      float scrollViewHeight = _height - (lastRectBeforeScrollView.y + lastRectBeforeScrollView.height) - 10;

      DrawLineInInspector(Color.black, 1, 5);

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width - 10), GUILayout.Height(scrollViewHeight));

      foreach (SearchListItem item in _items)
      {
        if (!ShouldBeVisible(item, anyTagChecked)) continue;

        EditorGUILayout.BeginHorizontal();

        //Add Image, if any.
        if (item.TagProvider != null && item.TagProvider.Aspect != null)
          GUILayout.Box(item.TagProvider.Aspect);
        //If there's no image, but other items have images, add a placeholder.
        else if (_someListItemsHaveTagsWithImages)
          GUILayout.Box(new Texture2D(_maxListItemTagWidth, _maxListItemTagHeight));

        //Configure Checkbox.
        List<GUILayoutOption> toggleLayoutOptions = new List<GUILayoutOption>();
        toggleLayoutOptions.Add(GUILayout.MaxWidth(15));
        //If we have images, make sure the checkbox is aligned with them.
        if (_someListItemsHaveTagsWithImages)
          toggleLayoutOptions.Add(GUILayout.Height(_maxListItemTagHeight + (_maxListItemTagHeight / 2)));
        //Add Checkbox.
        item.Checked = EditorGUILayout.Toggle(item.Checked, toggleLayoutOptions.ToArray());

        //Configure Label.
        List<GUILayoutOption> labelLayoutOptions = new List<GUILayoutOption>();
        //If we have images, make sure the label is aligned with them.
        if (_someListItemsHaveTagsWithImages)
          labelLayoutOptions.Add(GUILayout.Height(_maxListItemTagHeight + (_maxListItemTagHeight / 2)));
        //Add Label.
        EditorGUILayout.LabelField(item.Name, labelLayoutOptions.ToArray());

        //Next line!
        EditorGUILayout.EndHorizontal();
      }

      EditorGUILayout.EndScrollView();
    }

    private bool ShouldBeVisible(SearchListItem item, bool useTags)
    {
      if (_filterValue != "")
        if (item.Name.ToLower().IndexOf(_filterValue.ToLower()) == -1)
          return false;
      if (useTags && (item.TagProvider == null || !item.TagProvider.Checked))
        return false;
      return true;
    }

    static public void DrawLineInInspector(Color color, int thickness = 2, int padding = 10)
    {
      Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
      r.height = thickness;
      r.y += (float)padding / 2;
      r.x -= 2;
      r.width += 6;
      EditorGUI.DrawRect(r, color);
    }

  }

}