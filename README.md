# VolleyFightPFE

## Intentions
L'expérience cible est un match 2 contre 2 avec une intensité croissante.

## Comment jouer
Deux équipes s'affrontent, les bleus à gauche de l'écran et les jaunes à droite de l'écran.
Il faut attaquer la balle en faisant la touche attaque et en se plaçant face à la balle pour la toucher avec la zone rectangulaire (rouge=inactive, bleue=active).
La balle prend la couleur de la derniere équipe qui l'a touchée. Le filet central bloque la balle s'ils partagent la même couleur.
Les équipes doivent donc s'échanger la balle en essayant de se toucher directement. Si la balle touche un joueur, elle rebondit sur place et prend sa couleur.
La zone de block (sphere autour du joueur, rouge = inactive, verte = active) permet de faire rebondir la balle sans perdre de PV.
Chaque joueur peut se faire toucher 5 fois avant d'être éliminé.
Une fois tous les joueurs d'une équipe éliminés, l'équipe peut considérer être perdante.

Appuyer sur R pour recommencer un round (quand une équipe a perdu ou en cas de bug bloquant par exemple).

## Lancement du prototype
Dans la scène "TestBall", lancer la scène pour avoir un 2v2 avec 1 clavier et 3 manettes.
Pour changer les contrôles : sur les objets "PlayerX-Y" (X = l'équipe, Y = le numéro du joueur dans l'équipe), repérer les scripts "HumanPlayerInput" et "BotPlayerInput".
Le script "HumanPlayerInput" peut être activé et paramétré pour réagir au clavier (décocher IsGamepad) ou bien à une manette (cocher IsGamepad et indiquer un index, sachant que 0 = la première manette, 1 = la deuxième...).
Le script "BotPlayerInput" peut être activé pour faire jouer un bot automatiquement.
Ne pas activer en même temps les scripts "HumanPlayerInput" et "BotPlayerInput" sur le même joueur.

Pour jouer à moins de 4 joueurs, on peut juste désactiver les objets "PlayerX-Y" en trop.

## Contrôles

| Action        | Input (keyboard) | Input (controller) |
|---------------|------------------|--------------------|
| Move          | Z/Q/S/D          | Left Joystick      |
| (double) Jump | Space            | South Button       |
| Attack        | Left click       | West Button        |
| Block         | Right click      | East Button        |
| (Re)Start     | R                | /                  |

## Scène Blockout
Pour observer le blockout d'intention avec les lumières, lancer la scène "Blockout"