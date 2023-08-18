# Commandes customs
## Admin
#### .hordepresets
Retourne la liste des presets de horde disponibles (présents dans le fichier de config [Hordes.xml](Config/Hordes.xml))
#### .starthorde [preset]
Lance une horde s'il n'y en a pas déjà une en cours. Utilise le nom du preset passé s'il est présent, ou "default" s'il n'y en a pas.
#### .endhorde
Annule une horde s'il y en a une en cours.
#### .addsafezone
Lance un picker de tiles pour définir une nouvelle zone sécure. La première tile sélectionnée sera un coin, la deuxième le coin opposé (la zone sera toujours donc un rectangle). La liste des zones est dans le fichier de configuration [SafeZones.xml](Config/SafeZones.xml).
#### .corruption
Retourne le niveau de corruption de la cible.
#### .setcorruption [montant]
Met le niveau de corruption de la cible. Se retrouve entre 0 et 99.
#### .increasecorruption [montant]
Augmente la corruption de la cible.
#### .decreasecorruption [montant]
Diminue la corruption de la cible.
## Joueurs
#### .roll [nombre]/[nom de skill]
Effectue un roll de d20.

Sans paramètre, va simplement retourner un nombre de 1 à 20 au hasard, les joueurs peuvent faire ce qu'ils veulent de cette information.

Avec un nombre, un check sera fait et retournera automatiquement un succès ou un échec. L'attente est que ce nombre soit aussi entre 1 et 20 (à 1 les chances de succès sont de 100%, et à 20 de 0%)

Avec un nom de skill, le check sera fait contre la valeur du skill. Un skill à 0 sera toujours un échec et un skill au maximum sera toujours un succès.
