using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindingUI : MonoBehaviour
{
    [Serializable]
    public class ActionRebind
    {
        public string actionName;    // nome exato da Action no InputActionAsset
        public string bindingName; // ex: "press" ou parte de composite: "left","up" etc.
        public bool partOfComposite;   // true se bindingName for parte de um composite
        public TextMeshProUGUI bindingText;     // referência ao Text que mostra a binding
        public Button rebindButton;  // botão pra iniciar rebind

        [NonSerialized] public int bindingIndex;
    }

    public GameObject pressAnyKeyPanel; // painel que aparece quando está esperando input
    public PlayerInput playerInput;         // seu PlayerInput
    public ActionRebind[] rebinds;          // lista de bindings que podem ser rebindados

    private void OnEnable()
    {
        pressAnyKeyPanel.SetActive(false);
        // Carrega overrides
        var json = PlayerPrefs.GetString("rebinds", "");
        if (!string.IsNullOrEmpty(json)) playerInput.actions.LoadBindingOverridesFromJson(json);

        foreach (var r in rebinds)
        {
            // calcula bindingIndex pelo nome
            var action = playerInput.actions[r.actionName];
            r.bindingIndex = action.bindings
            .Select((b, i) => new { binding = b, idx = i })
            .FirstOrDefault(x =>
                string.IsNullOrEmpty(r.bindingName)
                    ? !x.binding.isComposite && !x.binding.isPartOfComposite // simples
                    : x.binding.name.Equals(r.bindingName, StringComparison.OrdinalIgnoreCase) &&
                    x.binding.isPartOfComposite == r.partOfComposite // composite
            )
            ?.idx ?? -1;

            if (r.bindingIndex < 0)
                Debug.LogError($"Binding '{r.bindingName}' não encontrado em '{r.actionName}'");

            UpdateBindingDisplay(r);

            // evita captura errada de variável no loop
            var tmp = r;
            tmp.rebindButton.onClick.AddListener(() => StartRebind(tmp));
        }
    }

    private void OnDisable()
    {
        foreach (var r in rebinds)
            r.rebindButton.onClick.RemoveAllListeners();
    }

    private void UpdateBindingDisplay(ActionRebind r)
    {
        var action = playerInput.actions[r.actionName];
        r.bindingText.text = action.GetBindingDisplayString(r.bindingIndex);
    }

    private void StartRebind(ActionRebind r)
    {
        var action = playerInput.actions[r.actionName];
        int index = r.bindingIndex;
        if (index < 0) return;

        r.bindingText.text = "...";
        pressAnyKeyPanel.SetActive(true);
        r.rebindButton.interactable = false;

        action.PerformInteractiveRebinding(index)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation =>
            {
                operation.Dispose();
                UpdateBindingDisplay(r);
                r.rebindButton.interactable = true;
                pressAnyKeyPanel.SetActive(false);

                // salva todas as overrides
                var json = playerInput.actions.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString("rebinds", json);
                PlayerPrefs.Save();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                UpdateBindingDisplay(r);
                r.rebindButton.interactable = true;
                pressAnyKeyPanel.SetActive(false);
            })
            .Start();
    }
}