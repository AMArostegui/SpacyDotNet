import spacy

nlp = spacy.load("en_core_web_sm")
doc = nlp("Apple is looking at buying U.K. startup for $1 billion")

for token in doc:
    print(token.text, token.lemma_, token.pos_, token.tag_, token.dep_,
          token.shape_, token.is_alpha, token.is_stop)

print("")
for ent in doc.ents:
    print(ent.text, ent.start_char, ent.end_char, ent.label_)

nlp = spacy.load("en_core_web_md")
tokens = nlp("dog cat banana afskfsd")

print("")
for token in tokens:
    print(token.text, token.has_vector, token.vector_norm, token.is_oov)